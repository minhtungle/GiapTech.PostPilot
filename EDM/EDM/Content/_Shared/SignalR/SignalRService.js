class SignalRService {
    constructor() {
    }
    init() {
        $.connection.hub.start().done();
    }
}