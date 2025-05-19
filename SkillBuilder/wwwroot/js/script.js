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

    window.openModal = function (modalId) {
        overlay.style.display = "block";
        document.getElementById(modalId).style.display = "block";
        navButtons.forEach(btn => btn.style.pointerEvents = "none");
    };

    // Updated closeModal function with second param hideOverlay (default true)
    window.closeModal = function (modalId, hideOverlay = true) {
        document.getElementById(modalId).style.display = "none";

        if (hideOverlay) {
            overlay.style.display = "none";

            // Re-enable navbar interactions
            navButtons.forEach(btn => btn.style.pointerEvents = "auto");
        }

        // Reset forms
        if (modalId === "login-modal") {
            loginForm.reset();
        } else if (modalId === "signup-modal") {
            signupForm.reset();
        }
    };
});

function togglePasswordVisibility(id, icon) {
    const input = document.getElementById(id);
    const isPassword = input.type === "password";
    input.type = isPassword ? "text" : "password";
    icon.textContent = isPassword ? "👁️" : "👁️‍🗨️";
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
    closeModal('login-modal', false); // hide login modal but keep overlay
    openModal('signup-modal');        // open signup modal (overlay remains)
}

function openModal(id) {
    document.getElementById(id).style.display = 'block';
}

function closeModal(id) {
    document.getElementById(id).style.display = 'none';
}

function switchToLogin() {
    closeModal('signup-modal', false); // hide signup modal but keep overlay
    openModal('login-modal');           // open login modal (overlay remains)
}

function openModal(id) {
    document.getElementById(id).style.display = 'block';
}

function closeModal(id) {
    document.getElementById(id).style.display = 'none';
}