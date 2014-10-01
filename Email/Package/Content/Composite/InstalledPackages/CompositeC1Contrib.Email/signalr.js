$(function () {
    var hub = $.connection.mailHub;

    var body = $('body');
    var tbody = $('table tbody', body);

    var consoleId = body.data('consoleid');
    var entityToken = body.data('entitytoken');
    var view = body.data('view');
    var baseUrl = body.data('baseurl');
    var queue = body.data('queue');
    var template = body.data('template');

    var isRelevantToPage = function (message) {
        if (queue && queue !== message.QueueId) {
            return false;
        }

        if (template && template !== message.TemplateKey) {
            return false;
        }

        return true;
    };

    var updateParents = function () {
        hub.server.updateQueuesCount(consoleId).done(function () {
            MessageQueue.update();
        });
    };

    var appendMessageLog = function (messsage) {
        var row = createLogRow(messsage);

        tbody.prepend(row);

        updateParents();
    };

    var createLogRow = function (message) {
        var tr = $('<tr />');

        var subjectCell = $('<td />').text(message.Subject);
        var timeCell = $('<td />').text(message.TimeStamp);
        var templateCell = $('<td />').text(message.TemplateKey);

        var deleteCell = $('<td />').append($('<a />', {
            'class': 'delete',
            href: baseUrl + '&id=' + message.Id
        }).text('Delete').data('id', message.Id));

        var viewCell = $('<td />').append($('<a />', {
            href: 'view.aspx' + baseUrl + '&cmd=delete&id=' + message.Id
        }).text('View'));

        tr.append(subjectCell).append(timeCell).append(templateCell).append(deleteCell).append(viewCell);

        return tr;
    };

    var handleLogRowRemoved = function (a) {
        a.parents('tr').remove();

        updateParents();
    };

    hub.client.mailQueued = function (message) {
        if (view === "Queued" && isRelevantToPage(message)) {
            appendMessageLog(message);
        }
    };

    hub.client.mailSent = function (message) {
        if (view === "Sent" && isRelevantToPage(message)) {
            appendMessageLog(message);
        }
    };

    // Start the connection.
    $.connection.hub.start().done(function () {
        var server = hub.server;

        tbody.on('click', 'a.delete', function (e) {
            var a = $(this);
            var id = a.data('id');

            if (view == "Queued") {
                server.deleteQueuedMessage(id).done(function () {
                    handleLogRowRemoved(a);
                });
            } else if (view == "Sent") {
                server.deleteSentMessage(id).done(function () {
                    handleLogRowRemoved(a);
                });
            }

            e.preventDefault();
        });
    });
});