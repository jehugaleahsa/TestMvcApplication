var buildUrl = null;
var showModal = null;

$(function () {
    var baseUrl = $('#base-url').attr('data-base-url');
    
    buildUrl = function () {
        var array = $.makeArray(arguments);
        var path = baseUrl + array.join('/');
        return path;
    }

    var modal = $('#modal-common');
    var titleSection = $('#title', modal);
    var contentSection = $('#content', modal);

    showModal = function (title, content) {
        titleSection.html(title);
        contentSection.html(content);
        modal.modal('show');
    }
});