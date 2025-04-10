import { Transport, ESPLoader, HardReset } from './esptool_bundle.js';

const Status = {
    None: 0,
    Connected: 1,
    Listening: 2
};

let _dotNetObject;
let _status = Status.None;
let _transport;
let _port;
let _espLoader;
let _chip = null

const _espLoaderTerminal = {
    clean() {
        if (_dotNetObject != null) {
            //_dotNetObject.invokeMethodAsync('CleanCocnsole');
        }
    },
    writeLine(data) {
        if (_dotNetObject != null) {
            _dotNetObject.invokeMethodAsync('WriteLineToConsole', data);
        }
    },
    write(data) {
        if (_dotNetObject != null) {
            _dotNetObject.invokeMethodAsync('WriteToConsole', data);
        }
    },
};

export async function startListening(baudRate) {
    try {
        if (_port == null) {
            _port = await navigator.serial.requestPort();
            _transport = new Transport(_port, false);
        }
        await _transport.connect(baudRate);
        const readLoop = _transport.rawRead();
        const decoder = new TextDecoder();

        setStatus(Status.Listening);
        while (_status == Status.Listening) {
            const { value, done } = await readLoop.next();

            if (done || !value) {
                break;
            }
            const text = decoder.decode(value);
            if (_dotNetObject != null) {
                _dotNetObject.invokeMethodAsync('WriteToConsole', text);
            }
        }
    }
    catch (error) {
        log(error.message);
        await disconnect();
        cleanUp();
    }
}

export async function disconnect() {
    try {
        if (_transport) {
            await _transport.disconnect();
            await _transport.waitForUnlock(1500);
            log("disconnected");
        }
        setStatus(Status.None);
        cleanUp();
    }
    catch (error) {
        log(error.message);
        cleanUp();
    }
}

export async function connect(baudRate, timeoutMs) {
    try {
        if (_port == null) {
            _port = await navigator.serial.requestPort();
            _transport = new Transport(_port, false);
            console.log("add port");
        }

        const loaderOptions = {
            transport: _transport,
            baudrate: parseInt(baudRate),
            terminal: _espLoaderTerminal,
            debugLogging: false,
        };
        _espLoader = new ESPLoader(loaderOptions);

        const timeoutPromise = new Promise((_, reject) =>
            setTimeout(() => reject(new Error("Connection timed out")), timeoutMs)
        );

        _chip = await Promise.race([
            _espLoader.main(),
            timeoutPromise
        ]);

        setStatus(Status.Connected);

        log("Settings done for :" + _chip);
    }
    catch (error) {
        log(error.message);
        await disconnect();
        cleanUp();
    }
}

export async function flashFiles(files) {
    try {
        const flashOptions = {
            fileArray: files,
            flashSize: "keep",
            eraseAll: false,
            compress: true,
            //reportProgress: (fileIndex, written, total) => {
            //    const percent = ((written / total) * 100).toFixed(2);
            //    log(`File ${files[fileIndex].name}: ${percent}% completed`);
            //},
        };

        await _espLoader.writeFlash(flashOptions);
        await _espLoader.after();
    }
    catch (error) {
        log(error.message);
    }
}

export async function eraseFlash() {
    try {
        if (_espLoader) {
            await _espLoader.eraseFlash();
            log("Flash erased");
        }
    }
    catch (error) {
        log(error.message);
    }
}


//export async function hardReset() {
//    try {
//        if (_transport) {
//            const reset = new HardReset(_transport);
//            await reset.reset();
//            log("hard reset");
//        }
//    }
//    catch (error) {
//        log(error.message);
//    }
//}

export function registerDotNetObject(dotNetObject) {
    _dotNetObject = dotNetObject;
    //log("Registering DotNetObject");
}

function setStatus(status) {
    _status = status;
    if (_dotNetObject != null) {
        _dotNetObject.invokeMethodAsync('SetStatus', status);
    }
}

function log(message) {
    console.log(message);
    if (_dotNetObject != null) {
        _dotNetObject.invokeMethodAsync('WriteLineToConsole', message);
    }
    
}

function cleanUp() {
    _transport = null;
    _port = null;
    _espLoader = null;
    _chip = null
    setStatus(Status.None);
}
