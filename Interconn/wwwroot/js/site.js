// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".sidebar");
    const mainContent = document.querySelector(".main-content");
    const hamburger = document.getElementById("hamburger-btn");

    hamburger.addEventListener("click", function () {
        sidebar.classList.toggle("open");
        mainContent.classList.toggle("shift");
    });
});