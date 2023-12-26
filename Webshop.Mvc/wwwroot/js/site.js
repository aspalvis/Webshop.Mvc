// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

String.prototype.blameOnEmpty = function (title = null) {
    if (this) {
        const val = this.toString();
        if (!val) {
            Swal.fire({
                icon: 'error',
                title: 'Ooops...',
                text: "Please enter " + (title ?? "input value") + "!"
            });
            throw new Error((title ?? " Input value") + "has no value!");
        }
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Ooops...',
            text: "Please enter " + (title ?? "input value") + "!"
        });
        return new Error((title ?? " Input value") + "has no value!");
    }
};
