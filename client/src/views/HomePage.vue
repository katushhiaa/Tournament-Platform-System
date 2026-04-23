<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue';
import TournamentCard from '../components/TournamentCard.vue';
import SiteFooter from '../components/SiteFooter.vue';

import heroBg from '../assets/hero-bg.png';
import cs2Card from '../assets/cs2-card.png';
import tennisCard from '../assets/tennis-card.png';
import forzaCard from '../assets/forza-card.png';
import chessCard from '../assets/chess-card.png';
import organizersBg from '../assets/organizers-bg.png';
import playersBg from '../assets/players-bg.png';
import ctaBg from '../assets/cta-bg.png';
import organizersTitleIcon from '../assets/icons/organizers-title-icon.svg';
import playersTitleIcon from '../assets/icons/players-title-icon.svg';
import checkIconBlue from '../assets/icons/check-icon-blue.svg';
import checkIconGreen from '../assets/icons/check-icon-green.svg';
</script>

<template>
  <div class="home-page">
    <section class="hero" :style="{ backgroundImage: `url(${heroBg})` }">
      <div class="hero__overlay"></div>
      <AppHeader />

      <div class="hero__content">
        <h1 class="hero__title">TOURNAMENT PLATFORM</h1>
        <p class="hero__subtitle">
          A platform for creating and participating in tournaments for your favorite
          games. Compete with other players, organize your own events, and reach
          new heights.
        </p>
        <button class="hero__button">Get started</button>
      </div>
    </section>

    <section class="section section--tournaments">
      <div class="section__inner">
        <h2 class="section__title">Active tournaments</h2>
        <p class="section__subtitle">
          Choose an active tournament and join the competition right now.
        </p>

        <div class="tournaments-grid">
          <TournamentCard
            :image="cs2Card"
            title="Counter strike 2"
            type="Game"
            date="24.05.2026"
            time="18:00"
            participants="32/64"
          />
          <TournamentCard
            :image="tennisCard"
            title="Tennis tour"
            type="Tennis"
            date="28.06.2026"
            time="16:00"
            participants="6/6"
          />
          <TournamentCard
            :image="forzaCard"
            title="Forza horizon 5"
            type="Game"
            date="21.03.2026"
            time="20:00"
            participants="16/16"
          />
          <TournamentCard
            :image="chessCard"
            title="Chess tour"
            type="Chess"
            date="23.02.2026"
            time="15:00"
            participants="12/12"
          />
        </div>

        <div class="section__button-wrapper">
          <button class="main-button">View All Tournaments</button>
        </div>
      </div>
    </section>

    <section class="section section--purpose">
      <div class="section__inner">
        <h2 class="section__title">OUR PURPOSE</h2>
        <p class="section__subtitle section__subtitle--wide">
          Zvytyaha unites organizers and players, creating a convenient environment
          for hosting and participating in esports tournaments.
        </p>

        <div class="purpose-grid">
          <article
            class="purpose-card purpose-card--blue"
            :style="{ backgroundImage: `url(${organizersBg})` }"
          >
            <div class="purpose-card__overlay"></div>
            <div class="purpose-card__content">
              <div class="purpose-card__title-row">
                <img
                  :src="organizersTitleIcon"
                  alt="Organizers icon"
                  class="purpose-card__title-icon"
                />
                <h3>FOR ORGANIZERS</h3>
              </div>

              <ul>
                <li>
                  <img :src="checkIconBlue" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Tournament creation and management</span>
                </li>
                <li>
                  <img :src="checkIconBlue" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Setting up competition rules and formats</span>
                </li>
                <li>
                  <img :src="checkIconBlue" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Tracking participants and results</span>
                </li>
                <li>
                  <img :src="checkIconBlue" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Automating tournament draws</span>
                </li>
              </ul>
            </div>
          </article>

          <article
            class="purpose-card purpose-card--green"
            :style="{ backgroundImage: `url(${playersBg})` }"
          >
            <div class="purpose-card__overlay"></div>
            <div class="purpose-card__content">
              <div class="purpose-card__title-row">
                <img
                  :src="playersTitleIcon"
                  alt="Players icon"
                  class="purpose-card__title-icon"
                />
                <h3>FOR PLAYERS</h3>
              </div>

              <ul>
                <li>
                  <img :src="checkIconGreen" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Participate in tournaments for various games</span>
                </li>
                <li>
                  <img :src="checkIconGreen" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Track your results</span>
                </li>
                <li>
                  <img :src="checkIconGreen" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Compete with other players</span>
                </li>
                <li>
                  <img :src="checkIconGreen" alt="Check icon" class="purpose-card__list-icon" />
                  <span>Increase your rating and experience</span>
                </li>
              </ul>
            </div>
          </article>
        </div>
      </div>
    </section>

    <section class="cta" :style="{ backgroundImage: `url(${ctaBg})` }">
      <div class="cta__overlay"></div>
      <div class="cta__content">
        <h2>Ready to get started?</h2>
        <p>Join the platform today and start your path to victory.</p>
        <router-link to="/register" class="main-button main-button--link">
          Create an Account
        </router-link>
      </div>
    </section>

    <SiteFooter />
  </div>
</template>

<style scoped>
.home-page {
  background: #252e35;
  color: #fffcf2;
}

.hero {
  position: relative;
  min-height: 740px;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.hero__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(37, 46, 53, 0.2) 0%,
    rgba(37, 46, 53, 0.32) 40%,
    rgba(37, 46, 53, 0.58) 100%
  );
}

.hero__content {
  position: relative;
  z-index: 2;
  max-width: 900px;
  margin: 0 auto;
  padding: 180px 24px 140px;
  text-align: center;
}

.hero__title {
  margin: 0 0 18px;
  color: #fffcf2;
  font-size: 40px;
  font-weight: 900;
  line-height: 1.08;
  text-transform: uppercase;
}

.hero__subtitle {
  max-width: 760px;
  margin: 0 auto 81px;
  color: #fffcf2;
  font-size: 16px;
  font-weight: 500;
  line-height: 1.35;
}

.hero__button,
.main-button {
  border: 1px solid #ff9800;
  border-radius: 12px;
  background: #ff9800;
  color: #fffcf2;
  font-weight: 700;
  cursor: pointer;
  transition:
    background 0.2s ease,
    color 0.2s ease,
    border-color 0.2s ease,
    box-shadow 0.2s ease;
}

.hero__button:hover,
.main-button:hover {
  background: #ff9800;
  color: #fffcf2;
}

.hero__button:active,
.main-button:active {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
}

.hero__button:focus-visible,
.main-button:focus-visible {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
  outline: none;
}

.hero__button {
  min-width: 166px;
  height: 46px;
  font-size: 16px;
}

.section {
  padding: 72px 0 84px;
}

.section--tournaments {
  background: #3a444c;
}

.section--purpose {
  background: #252e35;
}

.section__inner {
  width: min(1224px, calc(100% - 64px));
  margin: 0 auto;
}

.section__title {
  margin: 0 0 10px;
  text-align: center;
  color: #fffcf2;
  font-size: 40px;
  font-weight: 800;
  line-height: 1.1;
  text-transform: uppercase;
}

.section__subtitle {
  margin: 0 auto 42px;
  text-align: center;
  color: #fffcf2;
  font-size: 16px;
  line-height: 1.45;
  opacity: 0.92;
}

.section__subtitle--wide {
  max-width: 980px;
  margin-bottom: 54px;
}

.tournaments-grid {
  display: grid;
  grid-template-columns: repeat(4, 230px);
  justify-content: center;
  gap: 32px;
}

.section__button-wrapper {
  display: flex;
  justify-content: center;
  margin-top: 40px;
}

.section__button-wrapper .main-button {
  width: 237px;
  height: 46px;
  font-size: 16px;
}

/* OUR PURPOSE */
.purpose-grid {
  display: grid;
  grid-template-columns: repeat(2, 506px);
  justify-content: center;
  gap: 64px;
  margin-top: 0;
}

.purpose-card {
  position: relative;
  overflow: hidden;
  width: 506px;
  min-height: 302px;
  border: 1px solid;
  border-radius: 22px;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.purpose-card--blue {
  border-color: #1531ce;
}

.purpose-card--green {
  border-color: #84c082;
}

.purpose-card__overlay {
  position: absolute;
  inset: 0;
}

.purpose-card--blue .purpose-card__overlay {
  background: linear-gradient(
    90deg,
    rgba(21, 49, 206, 0.42) 0%,
    rgba(21, 49, 206, 0.26) 55%,
    rgba(21, 49, 206, 0.08) 100%
  );
}

.purpose-card--green .purpose-card__overlay {
  background: linear-gradient(
    90deg,
    rgba(132, 192, 130, 0.26) 0%,
    rgba(132, 192, 130, 0.16) 55%,
    rgba(132, 192, 130, 0.06) 100%
  );
}

.purpose-card__content {
  position: relative;
  z-index: 1;
  padding: 34px 34px 30px;
}

.purpose-card__title-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 30px;
}

.purpose-card__title-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
  flex-shrink: 0;
}

.purpose-card__content h3 {
  margin: 0;
  color: #fffcf2;
  font-size: 20px;
  font-weight: 800;
  text-transform: uppercase;
}

.purpose-card__content ul {
  margin: 0;
  padding: 0;
  list-style: none;
}

.purpose-card__content li {
  display: flex;
  align-items: center;
  gap: 5px;
  margin-bottom: 23px;
  color: #fffcf2;
  font-size: 14px;
  line-height: 1.4;
}

.purpose-card__content li:last-child {
  margin-bottom: 0;
}

.purpose-card__list-icon {
  width: 18px;
  height: 18px;
  object-fit: contain;
  flex-shrink: 0;
}

.main-button--link {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
}

/* CTA */
.cta {
  position: relative;
  min-height: 260px;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.cta__overlay {
  position: absolute;
  inset: 0;
  background: rgba(21, 49, 206, 0.38);
}

.cta__content {
  position: relative;
  z-index: 1;
  max-width: 900px;
  margin: 0 auto;
  padding: 52px 24px;
  text-align: center;
}

.cta__content h2 {
  margin: 0 0 12px;
  color: #fffcf2;
  font-size: 40px;
  font-weight: 800;
  line-height: 1.2;
}

.cta__content p {
  margin: 0 0 28px;
  color: #fffcf2;
  font-size: 16px;
  line-height: 1.4;
}

.cta__content .main-button {
  width: 166px;
  height: 46px;
  font-size: 16px;
}

@media (max-width: 1200px) {
  .section__inner {
    width: min(100%, calc(100% - 40px));
  }

  .tournaments-grid {
    grid-template-columns: repeat(2, 230px);
  }

  .purpose-grid {
    grid-template-columns: 1fr;
    justify-items: center;
    gap: 32px;
  }
}

@media (max-width: 960px) {
  .hero {
    min-height: auto;
  }

  .hero__content {
    padding: 110px 20px 80px;
  }

  .hero__title {
    font-size: 42px;
  }

  .hero__subtitle {
    font-size: 18px;
  }

  .section {
    padding: 56px 0 64px;
  }

  .section__inner {
    width: min(100%, calc(100% - 32px));
  }

  .section__title {
    font-size: 32px;
  }

  .section__subtitle {
    margin-bottom: 32px;
    font-size: 16px;
  }

  .tournaments-grid {
    grid-template-columns: repeat(2, minmax(0, 230px));
    gap: 24px;
  }

  .purpose-card {
    width: 100%;
    max-width: 506px;
  }

  .cta__content h2 {
    font-size: 32px;
  }
}

@media (max-width: 640px) {
  .hero__content {
    padding: 96px 16px 64px;
  }

  .hero__title {
    font-size: 32px;
  }

  .hero__subtitle {
    font-size: 16px;
    line-height: 1.5;
  }

  .hero__button {
    min-width: 190px;
    height: 50px;
    font-size: 18px;
  }

  .section__title {
    font-size: 28px;
  }

  .tournaments-grid {
    grid-template-columns: 1fr;
    justify-items: center;
  }

  .purpose-grid {
    gap: 24px;
  }

  .purpose-card__content {
    padding: 28px 24px 24px;
  }

  .purpose-card__content h3 {
    font-size: 18px;
    margin-bottom: 24px;
  }

  .cta {
    min-height: auto;
  }

  .cta__content {
    padding: 40px 16px;
  }

  .cta__content h2 {
    font-size: 28px;
  }

  .cta__content p {
    font-size: 15px;
  }
}
</style>