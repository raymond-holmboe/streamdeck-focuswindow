document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");
    //checkResize(actionInfo.payload.settings);

    websocket.addEventListener('message', function (event) {
        console.log("Got message event!");

        // Received message from Stream Deck
        var jsonObj = JSON.parse(event.data);
        console.log(jsonObj);

        //if (jsonObj.event === 'sendToPropertyInspector') {
        //    var payload = jsonObj.payload;
        //    checkResize(payload);
        //}
        //else if (jsonObj.event === 'didReceiveSettings') {
        //    var payload = jsonObj.payload;
        //    checkResize(payload.settings);
        //}
    });
});


function getChildWindows() {
    var payload = {};
    payload.property_inspector = 'getChildWindows';
    sendPayloadToPlugin(payload);
}

function getProcesses() {
    var payload = {};
    payload.property_inspector = 'getProcesses';
    sendPayloadToPlugin(payload);
}
