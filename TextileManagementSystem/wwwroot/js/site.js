document.addEventListener("DOMContentLoaded", () => {
  setupSidebarMenus();
  setupTableFilters();
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
