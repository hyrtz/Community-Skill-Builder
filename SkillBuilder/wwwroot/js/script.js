window.addEventListener("scroll", () => {
    const sections = document.querySelectorAll("section");
    const navLinks = document.querySelectorAll(".nav-center a");

    let current = "";

    sections.forEach((section) => {
        const sectionTop = section.offsetTop - 100;
        const sectionHeight = section.clientHeight;
        if (pageYOffset >= sectionTop && pageYOffset < sectionTop + sectionHeight) {
            current = section.getAttribute("id");
        }
    });

    navLinks.forEach((link) => {
        link.classList.remove("active");
        if (link.getAttribute("href") === `#${current}`) {
            link.classList.add("active");
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const overlay = document.getElementById("modal-overlay");
    const loginModal = document.getElementById("login-modal");
    const signupModal = document.getElementById("signup-modal");
    const loginForm = document.getElementById("login-form");
    const signupForm = document.getElementById("signup-form");

    const navButtons = document.querySelectorAll(".nav-buttons a");

    document.querySelector(".nav-buttons-login").addEventListener("click", function (e) {
        e.preventDefault();
        openModal("login-modal");
    });

    document.querySelector(".nav-buttons-signup").addEventListener("click", function (e) {
        e.preventDefault();
        openModal("signup-modal");
    });

    function openModal(modalId) {
        overlay.style.display = "block";
        document.getElementById(modalId).style.display = "block";

        // Disable navbar interactions
        navButtons.forEach(btn => btn.style.pointerEvents = "none");
    }

    window.closeModal = function (modalId) {
        document.getElementById(modalId).style.display = "none";
        overlay.style.display = "none";

        // Reset forms
        if (modalId === "login-modal") {
            loginForm.reset();
        } else if (modalId === "signup-modal") {
            signupForm.reset();
        }

        // Re-enable navbar interactions
        navButtons.forEach(btn => btn.style.pointerEvents = "auto");
    };
});

function togglePasswordVisibility(id, icon) {
    const input = document.getElementById(id);
    const isPassword = input.type === "password";
    input.type = isPassword ? "text" : "password";
    icon.textContent = isPassword ? "🙈" : "👁️";
}

function checkLoginInputs() {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    const button = document.querySelector('.login-btn');

    if (email.trim() && password.trim()) {
        button.disabled = false;
        button.classList.add('enabled');
    } else {
        button.disabled = true;
        button.classList.remove('enabled');
    }
}

function switchToSignup() {
    closeModal('login-modal');
    openModal('signup-modal');
}