<template>
  <footer class="footer">
    <div class="footer__top">
      <div class="footer__brand">
        <div class="footer__logo-row">
          <img src="../assets/logo-icon.png" alt="Zvytyaha logo" class="header__logo" />
          <span class="footer__title">ZVYTYAHA</span>
        </div>

        <p class="footer__description">
          Zvytyaha is a platform for organizing and participating in tournaments.
        </p>

         <div class="footer__socials">
            <a href="#" class="footer__social-link" aria-label="Instagram">
                <img
                src="../assets/icons/instagram-icon.svg"
                alt="Instagram"
                class="footer__social-icon"
                />
            </a>

            <a href="#" class="footer__social-link" aria-label="Discord">
                <img
                src="../assets/icons/discord-icon.svg"
                alt="Discord"
                class="footer__social-icon"
                />
            </a>

            <a href="#" class="footer__social-link" aria-label="Twitter">
                <img
                src="../assets/icons/twitter-icon.svg"
                alt="Twitter"
                class="footer__social-icon"
                />
            </a>
         </div>
      </div>

      <div class="footer__columns">
        <div class="footer__column">
          <h4>Product</h4>
          <a href="#">Features</a>
          <a href="#">Pricing</a>
          <a href="#">FAQ</a>
        </div>

        <div class="footer__column">
          <h4>Social</h4>
          <a href="#">Twitter</a>
          <a href="#">Discord</a>
          <a href="#">Instagram</a>
        </div>

        <div class="footer__column">
          <h4>Help</h4>
          <a href="#">Help Center</a>
          <a href="#">Contact Us</a>
        </div>
      </div>
    </div>

    <div class="footer__bottom">
      © 2026 Zvytyaha. All rights reserved.
    </div>
  </footer>
</template>

<script setup lang="ts"></script>

<style scoped>
.header__logo {
  width: 30px;
  height: 30px;
  object-fit: contain;
  flex-shrink: 0;
}

.footer {
  padding: 48px 84px 28px;
  background: #252e35;
  border-top: 1px solid rgba(21, 49, 206, 0.55);
}

.footer__top {
  display: flex;
  justify-content: space-between;
  gap: 60px;
  padding-bottom: 34px;
  border-bottom: 1px solid rgba(21, 49, 206, 0.45);
}

.footer__brand {
  max-width: 310px;
}

.footer__logo-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 22px;
}

.footer__logo {
  color: #ff9800;
}

.footer__title {
  color: #fffcf2;
  font-size: 16px;
  font-weight: 800;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.footer__description {
  margin: 0 0 24px;
  color: #fffcf2;
  font-size: 14px;
  line-height: 1.6;
  opacity: 0.9;
}

.footer__socials {
  display: flex;
  align-items: center;
  gap: 16px;
}

.footer__social-link {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
}

.footer__social-icon {
  width: 22px;
  height: 22px;
  object-fit: contain;
  display: block;
}

.footer__columns {
  display: flex;
  gap: 80px;
}

.footer__column {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.footer__column h4 {
  margin: 0 0 10px;
  color: #fffcf2;
  font-size: 24px;
  font-weight: 700;
}

.footer__column a {
  color: #fffcf2;
  font-size: 14px;
  text-decoration: none;
  opacity: 0.9;
}

.footer__bottom {
  padding-top: 20px;
  text-align: center;
  color: #fffcf2;
  font-size: 13px;
  opacity: 0.9;
}

@media (max-width: 960px) {
  .footer {
    padding: 32px 24px 24px;
  }

  .footer__top {
    flex-direction: column;
  }

  .footer__columns {
    gap: 32px;
    flex-wrap: wrap;
  }
}
</style>