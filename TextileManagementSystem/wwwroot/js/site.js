document.addEventListener("DOMContentLoaded", () => {
  setupSidebarMenus();
  setupTableFilters();
  setupInvoiceBuilder();
  setupOrderBuilder();
});

function setupSidebarMenus() {
  document.querySelectorAll("[data-menu-toggle]").forEach((toggle) => {
    toggle.addEventListener("click", (event) => {
      event.preventDefault();

      const group = toggle.closest("[data-menu-group]");
      const isOpen = group.classList.contains("open");

      document.querySelectorAll("[data-menu-group].open").forEach((openGroup) => {
        if (openGroup !== group) {
          openGroup.classList.remove("open");
          openGroup.querySelector("[data-menu-toggle]")?.setAttribute("aria-expanded", "false");
        }
      });

      group.classList.toggle("open", !isOpen);
      toggle.setAttribute("aria-expanded", String(!isOpen));
    });
  });
}

function setupTableFilters() {
  const globalSearch = document.querySelector("[data-global-search]");

  document.querySelectorAll("[data-filter-scope]").forEach((scope) => {
    const tableSearch = scope.querySelector("[data-table-search]");
    const materialFilter = scope.querySelector("[data-material-filter]");
    const statusFilter = scope.querySelector("[data-status-filter]");
    const resetButton = scope.querySelector("[data-reset-filters]");
    const exportButton = scope.querySelector("[data-export-csv]");
    const pills = scope.querySelectorAll("[data-filter-pills] [data-filter-value]");

    const apply = () => applyTableFilter(scope, {
      search: [globalSearch?.value, tableSearch?.value].filter(Boolean).join(" "),
      category: getActivePillValue(scope),
      material: materialFilter?.value ?? "all",
      status: statusFilter?.value ?? "all"
    });

    tableSearch?.addEventListener("input", apply);
    materialFilter?.addEventListener("change", apply);
    statusFilter?.addEventListener("change", apply);

    pills.forEach((pill) => {
      pill.addEventListener("click", () => {
        pills.forEach((item) => item.classList.remove("active"));
        pill.classList.add("active");
        apply();
      });
    });

    resetButton?.addEventListener("click", () => {
      if (tableSearch) {
        tableSearch.value = "";
      }

      if (globalSearch) {
        globalSearch.value = "";
      }

      if (materialFilter) {
        materialFilter.value = "all";
      }

      if (statusFilter) {
        statusFilter.value = "all";
      }

      pills.forEach((item) => item.classList.toggle("active", item.dataset.filterValue === "all"));
      apply();
    });

    exportButton?.addEventListener("click", () => {
      exportVisibleRows(scope, exportButton.dataset.exportCsv || "export.csv");
    });

    globalSearch?.addEventListener("input", apply);
    apply();
  });
}

function getActivePillValue(scope) {
  return scope.querySelector("[data-filter-pills] [data-filter-value].active")?.dataset.filterValue ?? "all";
}

function applyTableFilter(scope, filters) {
  const rows = [...scope.querySelectorAll("[data-filter-row]")];
  const noResults = scope.querySelector("[data-no-results]");
  const visibleCountLabel = scope.querySelector("[data-visible-count]");
  const searchTerms = normalize(filters.search).split(/\s+/).filter(Boolean);
  let visibleCount = 0;

  rows.forEach((row) => {
    const text = normalize(row.dataset.search);
    const category = normalize(row.dataset.category);
    const status = normalize(row.dataset.status);

    const matchesSearch = searchTerms.every((term) => text.includes(term));
    const matchesCategory = filters.category === "all" || category.includes(normalize(filters.category));
    const matchesMaterial = filters.material === "all" || category.includes(normalize(filters.material));
    const matchesStatus = filters.status === "all" || status.includes(normalize(filters.status));
    const visible = matchesSearch && matchesCategory && matchesMaterial && matchesStatus;

    row.classList.toggle("d-none", !visible);
    if (visible) {
      visibleCount += 1;
    }
  });

  noResults?.classList.toggle("d-none", visibleCount > 0 || rows.length === 0);
  if (visibleCountLabel) {
    visibleCountLabel.textContent = visibleCount.toString();
  }
}

function normalize(value) {
  return (value ?? "").toString().trim().toLowerCase();
}

function exportVisibleRows(scope, fileName) {
  const rows = [...scope.querySelectorAll("[data-filter-row]:not(.d-none)")];
  if (rows.length === 0) {
    return;
  }

  const csv = rows
    .map((row) => row.dataset.export || [...row.cells].map((cell) => cell.innerText.trim()).join(","))
    .map((line) => line.split(",").map(escapeCsvCell).join(","))
    .join("\n");

  const blob = new Blob([csv], { type: "text/csv;charset=utf-8" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
}

function escapeCsvCell(value) {
  const cell = value.trim();
  return /[",\n]/.test(cell) ? `"${cell.replaceAll('"', '""')}"` : cell;
}

function setupInvoiceBuilder() {
  const form = document.querySelector("[data-invoice-form]");
  if (!form) {
    return;
  }

  const body = form.querySelector("[data-line-items]");
  const addButton = form.querySelector("[data-add-line]");
  const money = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

  const renumberRows = () => {
    body.querySelectorAll("[data-line-row]").forEach((row, index) => {
      row.querySelectorAll("input").forEach((input) => {
        input.name = input.name.replace(/LineItems\[\d+\]/, `LineItems[${index}]`);
        input.id = input.id.replace(/LineItems_\d+__/, `LineItems_${index}__`);
      });
    });
  };

  const calculate = () => {
    let subtotal = 0;

    body.querySelectorAll("[data-line-row]").forEach((row) => {
      const qty = Number.parseFloat(row.querySelector("[data-line-qty]")?.value || "0");
      const rate = Number.parseFloat(row.querySelector("[data-line-rate]")?.value || "0");
      const discount = Number.parseFloat(row.querySelector("[data-line-discount]")?.value || "0");
      const lineTotal = Math.max(0, qty * rate * (1 - discount / 100));

      subtotal += lineTotal;
      row.querySelector("[data-line-total]").textContent = money.format(lineTotal);
    });

    const taxRate = Number.parseFloat(form.querySelector("[data-tax-rate]")?.value || "0");
    const tax = subtotal * (taxRate / 100);
    const gross = subtotal + tax;
    const rounded = Math.round(gross);
    const roundOff = rounded - gross;

    form.querySelector("[data-subtotal]").textContent = money.format(subtotal);
    form.querySelector("[data-tax]").textContent = money.format(tax);
    form.querySelector("[data-round]").textContent = money.format(roundOff);
    form.querySelector("[data-grand-total]").textContent = money.format(rounded);
  };

  addButton?.addEventListener("click", () => {
    const template = body.querySelector("[data-line-row]");
    const row = template.cloneNode(true);

    row.querySelectorAll("input").forEach((input) => {
      if (!input.matches("[data-line-qty]")) {
        input.value = input.matches("[data-line-discount]") ? "0" : "";
      }
    });

    row.querySelector("[data-line-total]").textContent = money.format(0);
    body.appendChild(row);
    renumberRows();
    calculate();
  });

  form.addEventListener("input", (event) => {
    if (event.target.closest("[data-line-row]") || event.target.matches("[data-tax-rate]")) {
      calculate();
    }
  });

  calculate();
}

function setupOrderBuilder() {
  const qty = document.querySelector("[data-order-qty]");
  const rate = document.querySelector("[data-order-rate]");
  const total = document.querySelector("[data-order-total]");
  if (!qty || !rate || !total) {
    return;
  }

  const money = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });
  const calculate = () => {
    const quantity = Number.parseFloat(qty.value || "0");
    const unitRate = Number.parseFloat(rate.value || "0");
    total.textContent = money.format(Math.max(0, quantity * unitRate));
  };

  qty.addEventListener("input", calculate);
  rate.addEventListener("input", calculate);
  calculate();
}
