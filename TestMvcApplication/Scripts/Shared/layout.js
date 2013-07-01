var buildUrl = null;
var showInformationModal = null;
var showConfirmationModal = null;

$(function () {
    var baseUrl = $('#base-url').attr('data-base-url');

    buildUrl = function () {
        var array = $.makeArray(arguments);
        var path = baseUrl + array.join('/');
        return path;
    }

    var informationModal = $('#modal-information');
    var informationTitle = $('#information-title', informationModal);
    var informationContent = $('#information-content', informationModal);

    showInformationModal = function (title, content) {
        informationTitle.html(title);
        informationContent.html(content);
        informationModal.modal('show');
    };

    var confirmationModal = $('#modal-confirmation');
    var confirmationContent = $('#confirmation-content', confirmationModal);
    var confirmationOK = $('#confirmation-ok', confirmationModal);

    showConfirmationModal = function (config) {
        confirmationContent.html(config.content);
        confirmationOK.removeClass();
        confirmationOK.addClass('btn');
        if (config.style) {
            confirmationOK.addClass(config.style);
        }
        confirmationOK.unbind('click');
        confirmationOK.click(config.success);
        confirmationModal.modal('show');
    };
});