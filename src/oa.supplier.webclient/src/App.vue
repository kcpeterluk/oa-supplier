<script setup lang="ts">
import { computed, ref } from "vue";

const appName = "OA Supplier";

type LoginState = "idle" | "success" | "error";
type SupplierFetchState = "idle" | "success" | "empty" | "error";

interface LoginResponse {
  accessToken?: string;
}

interface ApiResponse<T> {
  data: T;
}

interface SupplierRate {
  id: number;
  supplierId: number;
  rate: number;
  rateStartDate: string;
  rateEndDate: string | null;
  createdByUser: string;
  createdOn: string;
}

interface SupplierWithRates {
  id: number;
  name: string;
  address: string;
  createdByUser: string;
  createdOn: string;
  supplierRates: SupplierRate[];
}

const username = ref("");
const password = ref("");
const isLoading = ref(false);
const accessToken = ref("");
const loginState = ref<LoginState>("idle");
const suppliers = ref<SupplierWithRates[]>([]);
const isFetchingSuppliers = ref(false);
const supplierFetchState = ref<SupplierFetchState>("idle");

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

const supplierFetchMessage = computed(() => {
  if (isFetchingSuppliers.value) {
    return "Retrieving suppliers...";
  }

  if (supplierFetchState.value === "success") {
    return `Retrieved ${suppliers.value.length} supplier${suppliers.value.length === 1 ? "" : "s"}.`;
  }

  if (supplierFetchState.value === "empty") {
    return "No suppliers were returned.";
  }

  if (supplierFetchState.value === "error") {
    return "Could not retrieve suppliers. Please try again.";
  }

  return "";
});

const canFetchSuppliers = computed(() =>
  Boolean(accessToken.value) && !isLoading.value && !isFetchingSuppliers.value
);

function resetSuppliers() {
  suppliers.value = [];
  supplierFetchState.value = "idle";
}

function formatDate(value: string | null) {
  return value ?? "Open ended";
}

async function logIn() {
  isLoading.value = true;
  loginState.value = "idle";
  accessToken.value = "";
  resetSuppliers();

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
      resetSuppliers();
      return;
    }

    const data = (await response.json()) as LoginResponse;
    accessToken.value = data.accessToken ?? "";
    loginState.value = "success";
  } catch {
    loginState.value = "error";
    resetSuppliers();
  } finally {
    isLoading.value = false;
  }
}

async function fetchSuppliers() {
  if (!accessToken.value) {
    supplierFetchState.value = "error";
    suppliers.value = [];
    return;
  }

  isFetchingSuppliers.value = true;
  supplierFetchState.value = "idle";
  suppliers.value = [];

  try {
    const response = await fetch("/api/suppliers/", {
      method: "GET",
      headers: {
        Accept: "application/json",
        Authorization: `Bearer ${accessToken.value}`
      }
    });

    if (!response.ok) {
      supplierFetchState.value = "error";
      return;
    }

    const payload = (await response.json()) as ApiResponse<SupplierWithRates[]>;
    suppliers.value = payload.data ?? [];
    supplierFetchState.value = suppliers.value.length > 0 ? "success" : "empty";
  } catch {
    supplierFetchState.value = "error";
  } finally {
    isFetchingSuppliers.value = false;
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

      <section class="supplier-panel" aria-labelledby="supplier-title">
        <div class="supplier-actions">
          <h2 id="supplier-title">Suppliers and rates</h2>
          <button
            type="button"
            :disabled="!canFetchSuppliers"
            @click="fetchSuppliers"
          >
            {{ isFetchingSuppliers ? "Retrieving..." : "Get suppliers" }}
          </button>
        </div>

        <p
          v-if="supplierFetchMessage"
          class="supplier-message"
          :class="{
            success: supplierFetchState === 'success',
            error: supplierFetchState === 'error',
            empty: supplierFetchState === 'empty'
          }"
          role="status"
        >
          {{ supplierFetchMessage }}
        </p>

        <div v-if="suppliers.length" class="supplier-list">
          <article
            v-for="supplier in suppliers"
            :key="supplier.id"
            class="supplier-card"
          >
            <header class="supplier-card-header">
              <div>
                <h3>{{ supplier.name }}</h3>
                <p>{{ supplier.address }}</p>
              </div>
              <span>{{ supplier.supplierRates.length }} rate{{ supplier.supplierRates.length === 1 ? "" : "s" }}</span>
            </header>

            <div v-if="supplier.supplierRates.length" class="rate-list">
              <div class="rate-row rate-heading" aria-hidden="true">
                <span>Rate</span>
                <span>Start</span>
                <span>End</span>
              </div>

              <div
                v-for="rate in supplier.supplierRates"
                :key="rate.id"
                class="rate-row"
              >
                <span>{{ rate.rate }}</span>
                <span>{{ formatDate(rate.rateStartDate) }}</span>
                <span>{{ formatDate(rate.rateEndDate) }}</span>
              </div>
            </div>

            <p v-else class="no-rates">No rates for this supplier.</p>
          </article>
        </div>
      </section>
    </section>
  </main>
</template>
