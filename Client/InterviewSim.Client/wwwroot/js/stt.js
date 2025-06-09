export function start(dotNetRef) {
    const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
    if (!SpeechRecognition) {
        console.warn('Speech recognition not supported');
        return;
    }
    const recognition = new SpeechRecognition();
    window._sttRecognition = recognition;
    recognition.continuous = true;
    recognition.interimResults = false;
    recognition.onstart = () => dotNetRef.invokeMethodAsync('OnStart');
    recognition.onend = () => dotNetRef.invokeMethodAsync('OnEnd');
    recognition.onerror = (e) => dotNetRef.invokeMethodAsync('OnError', e.error);
    recognition.onresult = (e) => {
        let text = '';
        for (let i = e.resultIndex; i < e.results.length; i++) {
            text += e.results[i][0].transcript;
        }
        dotNetRef.invokeMethodAsync('OnResult', text);
    };
    recognition.start();
}

export function stop() {
    if (window._sttRecognition) {
        window._sttRecognition.stop();
    }
}
