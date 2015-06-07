
$(function () {

    // More Information (Ticket)

    $(".ticketInformation").click(function () {
        var type = $(this).attr("data-type");
        var path = "/Tickets/GetDetails";

        // Get the id from the link
        var recordToChange = $(this).attr("data-id");

        if (recordToChange != '') {
            // Perform the ajax post

            $.post(path, { "id": recordToChange },
                function (data) {
                    // Successful requests get here, update elements
                    $('#modal-ticket-number').text(data.number);
                    $('#modal-ticket-desc').text(data.description);
                    $('#modal-ticket-progress').text(data.progress);
                    $('#modal-ticket-action').text(data.actionWith);
                    $('#modal-ticket-priority').text(data.priority);
                    $('#modal-ticket-dateCreated').text(data.dateCreated);
                    $('#modal-ticket-status').text(data.status);
                    $('#modal-ticket-notes').text(data.notes);
                    $('#ticketModal').modal('show');
                });
        }
    });

    // More Information (Customer)

    $(".customerInformation").click(function () {
        var type = $(this).attr("data-type");
        var path = "/Customers/GetDetails";

        // Get the id from the link
        var recordToChange = $(this).attr("data-id");

        if (recordToChange != '') {
            // Perform the ajax post

            $.post(path, { "id": recordToChange },
                function (data) {
                    // Successful requests get here, update elements
                    $('#modal-customer-name').text(data.name);
                    $('#modal-customer-active').text(data.active);
                    $('#modal-customer-servDuration').text(data.servDuration);
                    $('#modal-customer-servStartDate').text(data.startDate);
                    $('#modal-customer-servEndDate').text(data.endDate);
                    $('#modal-customer-mainContact').text(data.custContact);
                    $('#modal-customer-numOpTickets').text(data.numOpenTickets);
                    $('#modal-customer-numOpCust').text(data.openCust);
                    $('#modal-customer-numOpRW').text(data.openRW);
                    $('#modal-customer-numClosedTickets').text(data.numClosedTickets);
                    $('#modal-customer-rwContact').text(data.rwContact);
                    $('#modal-customer-rwResource').text(data.rwResource);
                    $('#modal-customer-notes').text(data.notes);
                    $('#customerModal').modal('show');
                });
        }
    });


    $(".closeTicket").click(function () {
        var type = $(this).attr("data-type");
        var path = "/Tickets/CloseTicket";

        // Get the id from the link
        var recordToChange = $(this).attr("data-id");

        if (recordToChange != '') {
            // Perform the ajax post

            $.post(path, { "id": recordToChange },
                function (data) {
                    // Successful requests get here, update elements
                    $('#modal-ticket-status').text(data.status);
                });
        }
    });

    $(".changeAction").click(function () {
        var type = $(this).attr("data-type");
        var path = "/Tickets/ChangeAction";

        // Get the id from the link
        var recordToChange = $(this).attr("data-id");

        if (recordToChange != '') {
            // Perform the ajax post

            $.post(path, { "id": recordToChange },
                function (data) {
                    // Successful requests get here, update elements
                    $('.action-with-' + data.ticketId).text(data.action);
                });
        }
    });
});