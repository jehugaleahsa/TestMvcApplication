$(function () {
    $('#dpBirthDate').datepicker();
    defineValidation();
});

function defineValidation() {
    $('form').validate({
        rules: {
            txtName: {
                required: true
            },
            dpBirthDate: {
                required: true,
                date: true
            },
            txtHeight: {
                required: true,
                number: true
            }
        },
        messages: {
            txtName: {
                required: 'Please provide a name.'
            },
            dpBirthDate: {
                required: 'Please provide a date.',
                date: 'Please enter a valid date.'
            },
            txtHeight: {
                required: 'Please provide a height.',
                number: 'Please enter a valid number.'
            }
        },
        errorClass: 'error help-inline',
        validClass: 'success help-inline',
        highlight: function (element, errorClass, validClass) {
            $(element).parents('.control-group').addClass(errorClass).removeClass(validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parents('.control-group').addClass(validClass).removeClass(errorClass);
        }
    });
}