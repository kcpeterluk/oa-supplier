<script setup lang="ts">
import { computed, ref } from "vue";

const appName = "OA Supplier";

type LoginState = "idle" | "success" | "error";

interface LoginResponse {
  accessToken?: string;
}

const username = ref("");
const password = ref("");
const isLoading = ref(false);
const accessToken = ref("");
const loginState = ref<LoginState>("idle");

const loginMessage = computed(() => {
  if (loginState.value === "success") {
    return accessToken.value
      ? "Login successful."
      : "Login successful, but no access token was returned.";
  }

  if (loginState.value === "error") {
    return "Login failed. Please check your username and password.";
  }

  return "";
});

async function logIn() {
  isLoading.value = true;
  loginState.value = "idle";
  accessToken.value = "";

  try {
    const response = await fetch("/api/account/login?useCookies=false", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        email: username.value,
        password: password.value
      })
    });

    if (!response.ok) {
      loginState.value = "error";
      return;
    }

    const data = (await response.json()) as LoginResponse;
    accessToken.value = data.accessToken ?? "";
    loginState.value = "success";
  } catch {
    loginState.value = "error";
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <main class="shell">
    <section class="intro" aria-labelledby="page-title">
      <h1 id="page-title">{{ appName }}</h1>

      <form class="login-form" @submit.prevent="logIn">
        <label class="field">
          <span>Username</span>
          <input
            v-model="username"
            autocomplete="username"
            :disabled="isLoading"
            name="username"
            required
            type="text"
          />
        </label>

        <label class="field">
          <span>Password</span>
          <input
            v-model="password"
            autocomplete="current-password"
            :disabled="isLoading"
            name="password"
            required
            type="password"
          />
        </label>

        <button type="submit" :disabled="isLoading">
          {{ isLoading ? "Logging in..." : "Log in" }}
        </button>

        <p
          v-if="loginMessage"
          class="login-message"
          :class="{
            success: loginState === 'success',
            error: loginState === 'error'
          }"
          role="status"
        >
          {{ loginMessage }}
        </p>
      </form>
    </section>
  </main>
</template>
