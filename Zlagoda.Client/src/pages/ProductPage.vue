<template>
  <q-page padding>
    <div class="text-h5 q-mb-lg">
      <q-icon class="q-mr-md" :name="fasList" />Products
    </div>

    <div class="row">
      <div class="col-12 col-sm-10 col-lg-8">
        <q-table separator="horizontal"
                 :rows="filteredProducts"
                 :columns="columns"
                 :loading="loading"
                 :pagination="pagination"
                 binary-state-sort
                 no-data-label="No records exists"
                 no-results-label="No records found"
                 row-key="id_product">
          <template v-slot:top>
            <div class="row full-width">
              <q-input class="col-6 q-pa-sm"
                       dense
                       clearable
                       debounce="300"
                       placeholder="Filter"
                       v-model="filter.filter"
                       :disable="loading || product !== null">
                <template v-slot:append>
                  <q-icon :name="fasFilter" />
                </template>
              </q-input>
              <q-select class="col-6 q-pa-sm"
                        dense
                        clearable
                        placeholder="Select Category"
                        v-model="categoryFilter"
                        :options="categoryOptions"
                        :disable="loading || product !== null">
              </q-select>
            </div>
          </template>
          <template v-slot:body="props">
            <q-tr class="cursor-pointer"
                  :props="props"
                  @click="startEdit(props.row.id_product)">
              <template v-for="col in props.cols" :key="col.name">
                <q-td>{{ props.row[col.name] }}</q-td>
              </template>
            </q-tr>
          </template>
        </q-table>

        <q-page-sticky position="bottom-right"
                       :offset="$q.screen.name === 'xs' ? [9, 9] : [18, 18]">
          <q-btn color="accent"
                 :disable="loading || product !== null"
                 :icon="fasPlus"
                 @click="newProduct"
                 fab-mini="fab-mini"></q-btn>
        </q-page-sticky>
      </div>
    </div>

    <q-dialog v-model="dlg" persistent v-if="product !== null">
      <q-card class="q-dialog-plugin q-pa-md"
              style="width: 800px; max-width: 80vw"
              @click="animateOnEdit">
        <q-card-section>
          <div class="text-h6">
            {{ isNewProduct ? "New Product" : `Product ${product.id_product}` }}
          </div>
        </q-card-section>
        <q-card-section>
          <div class="row q-col-gutter-x-md">
            <div class="col-12 col-sm-6 col-lg-6">
              <q-input v-model="product.product_name"
                       label="Product Name"
                       dense
                       :rules="[(v) => v.length > 0 || 'Required']"></q-input>
            </div>
            <div class="col-12 col-sm-6 col-lg-6">
              <q-select v-model="product.category_number"
                        label="Category"
                        :options="categories"
                        dense
                        :rules="[(v) => v !== null || 'Required']"></q-select>
            </div>
            <div class="col-12 col-sm-6 col-lg-6">
              <q-input v-model="product.characteristics"
                       label="Characteristics"
                       dense
                       :rules="[(v) => v.length > 0 || 'Required']"></q-input>
            </div>
          </div>
        </q-card-section>
        <q-card-section>
          <div class="col-12 col-sm-4 col-lg-4 text-right">
            <q-btn v-if="product.product_name.length >
              0 &&
              product.category_number !== null &&
              product.characteristics.length > 0"
              @click="saveProduct"
              flat
              round
              :icon="fasCheck"
              color="positive">
            </q-btn>
            <q-btn @click="cancelEdit"
                   flat
                   round
                   :icon="fasXmark"></q-btn>
            <q-btn v-if="!isNewProduct"
                   @click="deleteProduct"
                   flat
                   round
                   :icon="fasTrash"
                   color="negative"></q-btn>
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>
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
    fasFilter
  } from "@quasar/extras/fontawesome-v6";

  defineOptions({
    name: "ProductPage",
  });

  const url = "product";
  const products = ref([]);
  const product = ref(null);
  const loading = ref(true);
  const filter = ref({
    filter: "",
  });
  const categoryFilter = ref(null);
  const pagination = ref({
    page: 1,
    rowsPerPage: 10,
    sortBy: "product_name",
    descending: false,
  });
  const columns = [
    {
      name: "product_name",
      label: "Product Name",
      field: "product_name",
      align: "left",
      sortable: true,
    },
    {
      name: "category_name",
      label: "Category Name",
      field: "category_name",
      align: "left",
      sortable: true,
    },
    {
      name: "category_number",
      label: "Category Number",
      field: "category_number",
      align: "left",
      sortable: true,
    },
    {
      name: "characteristics",
      label: "Characteristics",
      field: "characteristics",
      align: "left",
      sortable: false,
    }
  ];
  const categories = ref([]);
  const categoryOptions = computed(() => [
    { label: 'All Categories', value: null },
    ...categories.value
  ]);

  const newProduct = () => {
    product.value = {
      product_name: "",
      category_number: "",
      id_product: 0,
      characteristics: "",
    };
  };

  const startEdit = (id) => {
    const index = products.value.findIndex((el) => el.id_product === id);
    if (index > -1) {
      product.value = { ...products.value[index] };
    }
  };

  const cancelEdit = () => {
    product.value = null;
  };

  const saveProduct = () => {
    const req =
      product.value.id_product === 0
        ? api.post(url, product.value)
        : api.put(url, product.value);
    req
      .then((response) => {
        if (response.data === true || Number.isInteger(response.data)) {
          if (product.value.id_product === 0) {
            product.value.id_product = response.data;
            products.value.push(product.value);
          } else {
            const index = products.value.findIndex(
              (el) => el.id_product === product.value.id_product
            );
            if (index > -1) {
              products.value[index] = { ...product.value };
            }
          }
          product.value = null;
          notify.success("Save succeeded");
        } else notify.error("Cannot save product");
      })
      .catch((error) => {
        if (typeof error.response?.data === "string") {
          console.error(
            `SaveProduct: ${error.message}: ${error.response?.data}`
          );
          notify.error(`${error.message}: ${error.response?.data}`);
        } else {
          const msg =
            `\n${error.response?.data.title}\n` +
            Object.entries(error.response?.data?.errors).reduce(
              (s, [key, value]) => s + `${key}: ${value}\n`,
              ""
            );
          console.error(`SaveProduct: ${error.message}: ${msg}`);
          notify.error(`${error.message}: ${msg}`);
        }
      });
  };

  const removeProduct = (id) => {
    const index = products.value.findIndex((el) => el.id_product === id);
    if (index > -1) {
      products.value.splice(index, 1);
    }
  };

  const deleteProduct = () => {
    notify.confirm("Product will be deleted", "Are you sure?").onOk(() => {
      api
        .delete(`${url}/${product.value.id_product}`)
        .then((response) => {
          if (response.data) {
            removeProduct(product.value.id_product);
            product.value = null;
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

  const getProducts = () => {
    loading.value = true;
    api
      .get(url)
      .then((response) => {
        products.value = response.data;
      })
      .catch((error) => {
        notify.error(`Error: ${error.message}`);
      })
      .finally(() => (loading.value = false));
  };

  const getCategories = () => {
    api
      .get('categories')
      .then(response => {
        categories.value = response.data.map(category => ({
          label: category.category_name,
          value: category.category_number
        }));
      })
      .catch(error => {
        notify.error(`Error: ${error.message}`);
      });
  };

  const filteredProducts = computed(() => {
    return products.value.filter(product => {
      const matchesFilter = product.product_name.toLowerCase().includes(filter.value.filter.toLowerCase());
      const matchesCategory = categoryFilter.value === null || product.category_number === categoryFilter.value;
      return matchesFilter && matchesCategory;
    });
  });

  getProducts();
  getCategories();
</script>
