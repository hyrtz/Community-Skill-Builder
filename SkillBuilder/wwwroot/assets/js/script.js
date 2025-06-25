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
    // Element references
    const overlay = document.getElementById("modal-overlay");
    const loginForm = document.getElementById("login-form");
    const signupForm = document.getElementById("signup-form");
    const navButtons = document.querySelectorAll(".navbar-right a");

    const passwordInput = document.getElementById("signup-password");
    const confirmPasswordInput = document.getElementById("signup-confirm");
    const matchMessage = document.getElementById("match-message");
    const signupTrigger = document.querySelector(".nav-signup-btn");
    const signupSubmitBtn = document.getElementById("signup-submit-btn");
    const loginTrigger = document.querySelector(".nav-login-btn");
    const loginSubmitBtn = document.querySelector("#login-form button[type='submit']");

    const reqUppercase = document.getElementById("req-uppercase");
    const reqNumber = document.getElementById("req-number");
    const reqSymbol = document.getElementById("req-symbol");
    const reqLength = document.getElementById("req-length");

    const signupError = document.querySelector(".signup-error-message");
    const loginError = document.querySelector(".login-error-message");

    // Utility
    const isValidEmail = (email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    const isValidFirstName = (name) => /^[A-Za-z\s]{2,50}$/.test(name);
    const isValidLastName = (name) => /^[A-Za-z\s]{2,50}$/.test(name);


    // Reset signup view
    function resetSignupModalView() {
        const signupForm = document.getElementById("signup-form");

        signupForm.style.display = "none";
        document.querySelector(".social-login").style.display = "flex";
        document.querySelectorAll(".social-login button").forEach(btn => btn.style.display = "flex");
        document.querySelectorAll(".separator").forEach(s => s.style.display = "flex");
        document.querySelector(".signup-description").style.display = "block";
        document.querySelector(".continue-email-btn").style.display = "block";
        document.querySelector(".back-social-btn").style.display = "none";
    }

    window.resetSignupModalView = resetSignupModalView;

    function showSeparatorsInModal(modalId) {
        const modal = document.getElementById(modalId);
        modal?.querySelectorAll(".separator").forEach(s => s.style.display = "");
    }

    function showSocialLoginButtons() {
        document.querySelectorAll(".social-login button").forEach(btn => btn.style.display = "flex");
    }

    function focusFirstInput(modalId) {
        const modal = document.getElementById(modalId);
        const input = modal?.querySelector("input:not([type='hidden']):not([disabled])");
        if (input) input.focus();
    }

    window.openModal = function (modalId) {
        overlay.style.display = "block";
        document.getElementById(modalId).style.display = "block";
        navButtons.forEach(btn => btn.style.pointerEvents = "none");

        if (modalId === "signup-modal") {
            resetSignupModalView();
        }

        if (modalId === "login-modal") {
            loginForm.reset();
            if (loginError) loginError.textContent = "";
            showSeparatorsInModal("login-modal");
            showSocialLoginButtons();

            document.getElementById("login-main-content").style.display = "block";
            document.getElementById("forgot-password-form").style.display = "none";
        }

        focusFirstInput(modalId);
    };

    window.closeModal = function (modalId, hideOverlay = true) {
        const modal = document.getElementById(modalId);
        if (modal) modal.style.display = "none";

        if (hideOverlay) {
            overlay.style.display = "none";
            navButtons.forEach(btn => btn.style.pointerEvents = "auto");
        }

        if (modalId === "signup-modal") {
            signupForm.reset();
            resetSignupModalView();
            signupError.textContent = "";
        }
    };

    // Outside click close
    overlay.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    window.switchToSignup = () => {
        closeModal("login-modal", false);
        openModal("signup-modal");
    };

    window.switchToLogin = () => {
        closeModal("signup-modal", false);
        openModal("login-modal");
    };

    window.showLoginForm = function () {
        document.getElementById("forgot-password-form").style.display = "none";
        document.getElementById("login-main-content").style.display = "block";
    };

    window.showForgotPasswordForm = function (event) {
        event.preventDefault(); // Prevents anchor from jumping to top
        document.getElementById("login-main-content").style.display = "none";
        document.getElementById("forgot-password-form").style.display = "block";
    };

    window.showSignupForm = function () {
        signupForm.style.display = "block";
        document.querySelector(".social-login").style.display = "none";
        document.querySelectorAll(".social-login button").forEach(btn => btn.style.display = "none");
        document.querySelectorAll(".auth-modal-right .separator").forEach(s => s.style.display = "none");
        document.querySelector(".signup-description").style.display = "none";
        document.querySelector(".continue-email-btn").style.display = "none";
        document.querySelector(".back-social-btn").style.display = "inline-block";
    };

    window.togglePasswordVisibility = function (inputId, toggleIcon) {
        const input = document.getElementById(inputId);
        if (!input) return;

        input.type = input.type === "password" ? "text" : "password";
        toggleIcon.textContent = input.type === "text" ? "👁️" : "👁️‍🗨️";
    };

    window.checkLoginInputs = function () {
        const email = document.getElementById("login-email").value;
        const password = document.getElementById("login-password").value;
        loginSubmitBtn.disabled = !(email && password);
    };

    function checkSignupInputs() {
        const firstname = document.getElementById("signup-firstname").value.trim();
        const lastname = document.getElementById("signup-lastname").value.trim();
        const email = document.getElementById("signup-email").value.trim();
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;
        const agree = document.getElementById("signup-agree").checked;

        // Password match check
        const passwordsMatch = password === confirmPassword;
        matchMessage.style.display = confirmPassword.length > 0 && !passwordsMatch ? "block" : "none";

        const firstNameError = document.getElementById("firstname-error");
        const lastNameError = document.getElementById("lastname-error");

        const validFirstName = isValidFirstName(firstname);
        const validLastName = isValidLastName(lastname);

        // Show errors
        firstNameError.textContent = validFirstName ? "" : "First name must contain only letters.";
        lastNameError.textContent = validLastName ? "" : "Last name must contain only letters.";

        // Combine all validations
        const valid =
            validFirstName &&
            validLastName &&
            isValidEmail(email) &&
            /[A-Z]/.test(password) &&
            /\d/.test(password) &&
            /[!@#\^*_\-]/.test(password) &&
            password.length >= 8 &&
            passwordsMatch &&
            agree;

        signupSubmitBtn.disabled = !valid;

        validatePassword(); // still useful
    }

    function validatePassword() {
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;

        const passwordRequirements = document.getElementById("password-requirements");

        // Show requirements only if something is typed in the password
        if (password.length > 0) {
            passwordRequirements.style.display = "block";
        } else {
            passwordRequirements.style.display = "none";
        }

        // Update individual requirement lines
        updateRequirement("req-uppercase", /[A-Z]/.test(password));
        updateRequirement("req-number", /\d/.test(password));
        updateRequirement("req-symbol", /[!@#\^*_\-]/.test(password));
        updateRequirement("req-length", password.length >= 8);

        // Show password match warning only if confirm password has input
        if (confirmPassword.length > 0 && password !== confirmPassword) {
            matchMessage.style.display = "block";
        } else {
            matchMessage.style.display = "none";
        }

    }

    function updateRequirement(id, valid) {
        const el = document.getElementById(id);
        const baseText = el.dataset.text || el.textContent.replace(/^✔️|❌/, "").trim();
        el.textContent = (valid ? "✔️ " : "❌ ") + baseText;
        el.style.color = valid ? "green" : "orange";
    }

    function showEmailVerificationMessage(email) {
        const form = document.getElementById("signup-inputs-wrapper");
        const verificationMessage = document.getElementById("email-verification-message");
        const obfuscatedEmail = document.getElementById("obfuscated-email");
        const signupModal = document.getElementById("signup-modal");

        if (form) form.style.display = "none";
        if (verificationMessage) verificationMessage.style.display = "block";
        if (obfuscatedEmail) obfuscatedEmail.textContent = obfuscateEmail(email);

        // Only hide elements within signup modal
        const closeBtn = signupModal.querySelector(".auth-close-btn");
        const backBtn = signupModal.querySelector(".back-social-btn");
        const redirectLogin = signupModal.querySelector(".signup-redirect");

        if (closeBtn) closeBtn.style.display = "none";
        if (backBtn) backBtn.style.display = "none";
        if (redirectLogin) redirectLogin.style.display = "none";
    }

    // Optional helper to hide part of the email address
    function obfuscateEmail(email) {
        const [user, domain] = email.split("@");
        const maskedUser = user.length > 2
            ? user[0] + "*".repeat(user.length - 2) + user[user.length - 1]
            : "*".repeat(user.length);
        return `${maskedUser}@${domain}`;
    }

    ["signup-firstname", "signup-lastname", "signup-email", "signup-password", "signup-confirm"].forEach(id => {
        document.getElementById(id).addEventListener("input", checkSignupInputs);
    });

    const skipVerificationBtn = document.getElementById("skip-verification-btn");

    if (skipVerificationBtn) {
        skipVerificationBtn.addEventListener("click", function () {

            document.querySelectorAll("#signup-form input").forEach(input => {
                input.disabled = true;
            });

            document.querySelector(".nav-login-btn")?.style.setProperty("display", "none", "important");
            document.querySelector(".nav-signup-btn")?.style.setProperty("display", "none", "important");

            document.querySelector(".nav-notification-icon")?.style.setProperty("display", "inline-block", "important");
            document.querySelector(".nav-profile-icon")?.style.setProperty("display", "inline-block", "important");

            closeModal("signup-modal");
            window.location.reload();

            // openModal("interest-modal");
        });
    }

    document.getElementById("signup-agree").addEventListener("change", checkSignupInputs);

    let signupInProgress = false;

    signupForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        if (signupInProgress) return; // prevent double submission
        signupInProgress = true;

        signupError.textContent = "";

        const firstName = document.getElementById("signup-firstname").value.trim();
        const lastName = document.getElementById("signup-lastname").value.trim();
        const email = document.getElementById("signup-email").value.trim();
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;

        if (password !== confirmPassword) {
            signupError.textContent = "Passwords do not match.";
            signupInProgress = false;
            return;
        }

        signupSubmitBtn.disabled = true;
        signupSubmitBtn.textContent = "Creating account...";

        try {
            const response = await fetch("/signup", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    FirstName: firstName,
                    LastName: lastName,
                    Email: email,
                    Password: password
                }),
            });

            const data = await response.json().catch(() => ({ message: "Unexpected server response." }));

            if (response.ok) {
                const loginBtn = document.querySelector(".nav-login-btn");
                const signupBtn = document.querySelector(".nav-signup-btn");

                if (loginBtn) loginBtn.style.display = "none";
                if (signupBtn) signupBtn.style.display = "none";

                const notificationIcon = document.querySelector(".nav-notification-icon");
                const profileIcon = document.querySelector(".nav-profile-icon");

                if (notificationIcon) notificationIcon.style.display = "inline-block";
                if (profileIcon) profileIcon.style.display = "inline-block";

                document.body.classList.add("user-logged-in");

                const submitBtn = document.getElementById("signup-submit-btn");
                if (submitBtn) submitBtn.style.display = "none";

                showEmailVerificationMessage(email);
            } else {
                signupError.textContent = data.message || "Signup failed.";
            }

        } catch (error) {
            console.error("Signup error:", error);
            signupError.textContent = "An error occurred. Please try again later.";
        }

        signupSubmitBtn.disabled = false;
        signupSubmitBtn.textContent = "Create Account";
        signupInProgress = false;
    });

    let loginInProgress = false;

    // Handle login submission
    loginForm.addEventListener("submit", async function (e) {
        e.preventDefault();

        if (loginInProgress) return;
        loginInProgress = true;

        loginError.textContent = "";

        const email = document.getElementById("login-email").value.trim();
        const password = document.getElementById("login-password").value;

        loginSubmitBtn.disabled = true;
        loginSubmitBtn.textContent = "Logging in...";

        try {
            const response = await fetch("/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email: email, Password: password }),
            });

            const data = await response.json().catch(() => ({ message: "Unexpected server response." }));

            if (response.ok) {
                document.body.classList.add("user-logged-in");

                const loginBtn = document.querySelector(".nav-login-btn");
                const signupBtn = document.querySelector(".nav-signup-btn");
                const notificationIcon = document.querySelector(".nav-notification-icon");
                const profileIcon = document.querySelector(".nav-profile-icon");

                if (loginBtn) loginBtn.style.display = "none";
                if (signupBtn) signupBtn.style.display = "none";
                if (notificationIcon) notificationIcon.style.display = "inline-block";
                if (profileIcon) profileIcon.style.display = "inline-block";

                closeModal("login-modal");
                window.location.reload();

            } else {
                loginError.textContent = data.message || "Invalid login credentials.";
            }

        } catch (error) {
            console.error("Login error:", error);
            loginError.textContent = "An error occurred. Please try again later.";
        }

        loginSubmitBtn.disabled = false;
        loginSubmitBtn.textContent = "Log In";
        loginInProgress = false;
    });

    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get("verified") === "true") {
        // ✅ Close modal if open
        closeModal("signup-modal");

        // ✅ Show success toast
        const successPopup = document.createElement("div");
        successPopup.textContent = "🎉 Your email has been successfully verified!";
        successPopup.style.cssText = `
            position: fixed;
            top: 1rem;
            right: 1rem;
            background-color: #4BB543;
            color: white;
            padding: 1rem 1.5rem;
            border-radius: 8px;
            z-index: 9999;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
            font-size: 0.95rem;
        `;

        document.body.appendChild(successPopup);

        // Auto-dismiss after 4 seconds
        setTimeout(() => successPopup.remove(), 4000);

        // Clean up the URL
        const cleanUrl = window.location.origin + window.location.pathname;
        window.history.replaceState({}, document.title, cleanUrl);
    }

    const skipButton = document.getElementById("skip-interest-button");
    let skipClicked = false;

    if (skipButton) {
        skipButton.addEventListener("click", function () {
            if (skipClicked) return;
            skipClicked = true;

            // ✅ Disable required inputs to prevent "not focusable" error
            document.querySelectorAll("#signup-form input").forEach(input => {
                input.disabled = true;
            });

            // Hide login/signup buttons
            document.querySelector(".nav-login-btn")?.style.setProperty("display", "none", "important");
            document.querySelector(".nav-signup-btn")?.style.setProperty("display", "none", "important");

            // Show notification/profile
            document.querySelector(".nav-notification-icon")?.style.setProperty("display", "inline-block", "important");
            document.querySelector(".nav-profile-icon")?.style.setProperty("display", "inline-block", "important");

            closeModal("signup-modal");
            window.location.reload();
        });
    }

    if (loginTrigger) {
        loginTrigger.addEventListener("click", e => {
            e.preventDefault();
            openModal("login-modal");
        });
    }

    if (signupTrigger) {
        signupTrigger.addEventListener("click", e => {
            e.preventDefault();
            openModal("signup-modal");
        });
    }

    document.getElementById("login-email").addEventListener("input", checkLoginInputs);
    document.getElementById("login-password").addEventListener("input", checkLoginInputs);
});

// Escape key closes modal
document.addEventListener("keydown", function (e) {
    const emailMessageVisible = document.getElementById("email-verification-message")?.style.display === "block";
    if (e.key === "Escape" && !emailMessageVisible) {
        window.closeModal("login-modal");
        window.closeModal("signup-modal");
    }
});