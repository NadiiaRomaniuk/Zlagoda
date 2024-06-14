<template>
  <q-page padding>
    <div class="text-h5 q-mb-lg">
      <q-icon class="q-mr-md" :name="fasList" />Categories
    </div>

    <div class="row">
      <div class="col-12 col-sm-10 col-lg-8">
        <q-input
          v-model="filter"
          placeholder="Search categories"
          dense
          debounce="300"
          class="q-mb-md"
          clearable
          @input="updateFilter"
        />
        <q-table
          separator="cell"
          :rows="filteredCategories"
          :columns="columns"
          :loading="loading"
          :pagination="pagination"
          binary-state-sort
          row-key="categoryId"
        >
          <template v-slot:top>
            <div v-if="category?.categoryId === 0" class="row full-width">
              <div class="col-10">
                <q-input
                  v-model="category.name"
                  label="New category name"
                  autofocus
                  dense
                  class="q-px-sm"
                  :rules="[
                    (v) => v.length > 0 || 'Required',
                    (v) => v.length <= 50 || 'Too long',
                  ]"
                  @keyup.enter="saveCategory"
                  @keyup.escape="cancelEdit"
                />
              </div>
              <div class="col-2 row justify-end items-center">
                <q-btn
                  v-if="category.name.length > 0 && category.name.length <= 50"
                  @click="saveCategory"
                  title="Save"
                  flat
                  round
                  :icon="fasCheck"
                  color="positive"
                />
                <q-btn
                  @click="cancelEdit"
                  title="Cancel"
                  flat
                  round
                  :icon="fasXmark"
                />
              </div>
            </div>
          </template>
          <template v-slot:body="props">
            <q-tr
              v-if="category?.categoryId !== props.row.categoryId"
              @click="
                category !== null ? null : startEdit(props.row.categoryId)
              "
              :props="props"
              class="cursor-pointer"
              :disable="category !== null"
              :class="{ disabled: category !== null }"
            >
              <q-td colspan="2">{{ props.row.name }}</q-td>
            </q-tr>
            <q-tr v-else>
              <q-td colspan="2">
                <div class="row">
                  <div class="col-10">
                    <q-input
                      v-model="category.name"
                      label="Category name"
                      autofocus
                      dense
                      class="q-px-sm"
                      :rules="[
                        (v) => v.length > 0 || 'Required',
                        (v) => v.length <= 50 || 'Too long',
                      ]"
                      @keyup.enter="saveCategory"
                      @keyup.escape="cancelEdit"
                    />
                  </div>
                  <div class="col-2 row justify-end items-center">
                    <q-btn
                      v-if="
                        category.name.length > 0 && category.name.length <= 50
                      "
                      @click="saveCategory"
                      title="Save"
                      flat
                      round
                      :icon="fasCheck"
                      color="positive"
                    />
                    <q-btn
                      @click="cancelEdit"
                      title="Cancel"
                      flat
                      round
                      :icon="fasXmark"
                    />
                    <q-btn
                      @click="deleteCategory"
                      title="Delete"
                      flat
                      round
                      :icon="fasTrash"
                      color="negative"
                    />
                  </div>
                </div>
              </q-td>
            </q-tr>
          </template>
        </q-table>
      </div>
    </div>

    <q-page-sticky
      position="bottom-right"
      :offset="$q.screen.name === 'xs' ? [9, 9] : [18, 18]"
    >
      <q-btn
        color="accent"
        :disable="loading || category !== null"
        :icon="fasPlus"
        @click="newCategory"
        fab-mini="fab-mini"
      ></q-btn>
    </q-page-sticky>
  </q-page>
</template>

<script setup>
import { ref, computed } from "vue";
import { api } from "boot/axios";
import notify from "../notices";
import {
  fasList,
  fasCheck,
  fasXmark,
  fasTrash,
  fasPlus,
} from "@quasar/extras/fontawesome-v6";

defineOptions({
  name: "CategoryPage",
});

const url = "category";
const categories = ref([]);
const category = ref(null);
const loading = ref(true);
const filter = ref("");
const pagination = ref({
  page: 1,
  rowsPerPage: 10,
  sortBy: "name",
  descending: false,
});
const columns = [
  {
    name: "name",
    label: "Name",
    field: "name",
    align: "left",
    sortable: true,
  },
];

const newCategory = () => {
  category.value = {
    name: "",
    categoryId: 0,
  };
};

const startEdit = (id) => {
  const index = categories.value.findIndex((el) => el.categoryId === id);
  if (index > -1) {
    category.value = {
      name: categories.value[index].name,
      categoryId: categories.value[index].categoryId,
    };
  }
};

const cancelEdit = () => {
  category.value = null;
};

const saveCategory = () => {
  const req =
    category.value.categoryId === 0
      ? api.post(url, category.value)
      : api.put(url, category.value);
  req
    .then((response) => {
      if (response.data === true || Number.isInteger(response.data)) {
        if (category.value.categoryId === 0) {
          category.value.categoryId = response.data;
          categories.value.push(category.value);
        } else {
          const index = categories.value.findIndex(
            (el) => el.categoryId === category.value.categoryId
          );
          if (index > -1) {
            categories.value[index].name = category.value.name;
          }
        }
        category.value = null;
        notify.success("Save successed");
      } else notify.error("Cannot save category");
    })
    .catch((error) => {
      if (typeof error.response?.data === "string") {
        console.error(
          `SaveCategory: ${error.message}: ${error.response?.data}`
        );
        notify.error(`${error.message}: ${error.response?.data}`);
      } else {
        const msg =
          `\n${error.response?.data.title}\n` +
          Object.entries(error.response?.data?.errors).reduce(
            (s, [key, value]) => s + `${key}: ${value}\n`,
            ""
          );
        console.error(`SaveCategory: ${error.message}: ${msg}`);
        notify.error(`${error.message}: ${msg}`);
      }
    });
};

const removeCategory = (id) => {
  const index = categories.value.findIndex((el) => el.categoryId === id);
  if (index > -1) {
    categories.value.splice(index, 1);
  }
};

const deleteCategory = () => {
  notify.confirm("Category will be deleted", "Are you sure?").onOk(() => {
    api
      .delete(`${url}/${category.value.categoryId}`)
      .then((response) => {
        if (response.data) {
          removeCategory(category.value.categoryId);
          category.value = null;
          notify.success("Delete success");
        } else {
          notify.error("Delete error");
        }
      })
      .catch((error) => {
        notify.error(`Error: ${error.message}`);
      });
  });
};

const getCategories = () => {
  loading.value = true;
  api
    .get(url)
    .then((response) => {
      categories.value = response.data;
    })
    .catch((error) => {
      notify.error(`Error: ${error.message}`);
    })
    .finally(() => (loading.value = false));
};

const filteredCategories = computed(() => {
  return categories.value.filter(
    (category) =>
      filter.value === null ||
      category.name.toLowerCase().includes(filter.value.toLowerCase())
  );
});

const updateFilter = () => {
  pagination.value.page = 1;
};

getCategories();
</script>
