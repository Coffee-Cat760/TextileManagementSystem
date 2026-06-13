document.addEventListener("DOMContentLoaded", () => {
  setupSidebarMenus();
  setupTableFilters();
  setupPagination();
  setupProfileTabs();
  setupFileInputs();
  setupDownloadButtons();
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

    if (exportButton) {
      exportButton.dataset.exportBound = "true";
      exportButton.addEventListener("click", () => exportVisibleRows(scope, exportButton.dataset.exportCsv || "export.csv"));
    }

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
  const paginated = scope.matches("[data-paginate]");

  rows.forEach((row) => {
    const text = normalize(row.dataset.search);
    const category = normalize(row.dataset.category);
    const status = normalize(row.dataset.status);

    const matchesSearch = searchTerms.every((term) => text.includes(term));
    const matchesCategory = filters.category === "all" || category.includes(normalize(filters.category));
    const matchesMaterial = filters.material === "all" || category.includes(normalize(filters.material));
    const matchesStatus = filters.status === "all" || status.includes(normalize(filters.status));
    const visible = matchesSearch && matchesCategory && matchesMaterial && matchesStatus;

    row.dataset.filterHidden = visible ? "false" : "true";
    if (!paginated) {
      row.classList.toggle("d-none", !visible);
    }

    if (visible) {
      visibleCount += 1;
    }
  });

  noResults?.classList.toggle("d-none", visibleCount > 0 || rows.length === 0);
  if (visibleCountLabel) {
    visibleCountLabel.textContent = visibleCount.toString();
  }

  if (paginated) {
    scope.dataset.page = "1";
    updatePagination(scope);
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

function setupPagination() {
  document.querySelectorAll("[data-paginate]").forEach((scope) => {
    scope.dataset.page = scope.dataset.page || "1";
    scope.querySelector("[data-page-prev]")?.addEventListener("click", () => {
      scope.dataset.page = Math.max(1, Number(scope.dataset.page || 1) - 1).toString();
      updatePagination(scope);
    });

    scope.querySelector("[data-page-next]")?.addEventListener("click", () => {
      const totalPages = getTotalPages(scope);
      scope.dataset.page = Math.min(totalPages, Number(scope.dataset.page || 1) + 1).toString();
      updatePagination(scope);
    });

    updatePagination(scope);
  });
}

function updatePagination(scope) {
  const rows = [...scope.querySelectorAll("[data-page-row]")];
  const pageSize = Number(scope.dataset.pageSize || 10);
  const availableRows = rows.filter((row) => row.dataset.filterHidden !== "true");
  const totalPages = Math.max(1, Math.ceil(availableRows.length / pageSize));
  const page = Math.min(Math.max(1, Number(scope.dataset.page || 1)), totalPages);
  const start = (page - 1) * pageSize;
  const end = start + pageSize;

  scope.dataset.page = page.toString();
  rows.forEach((row) => {
    const index = availableRows.indexOf(row);
    const visible = index >= start && index < end;
    row.classList.toggle("d-none", !visible);
  });

  const label = scope.querySelector("[data-page-label]");
  if (label) {
    label.textContent = `${page} / ${totalPages}`;
  }

  const prev = scope.querySelector("[data-page-prev]");
  const next = scope.querySelector("[data-page-next]");
  if (prev) {
    prev.disabled = page <= 1;
  }

  if (next) {
    next.disabled = page >= totalPages;
  }
}

function getTotalPages(scope) {
  const pageSize = Number(scope.dataset.pageSize || 10);
  const rows = [...scope.querySelectorAll("[data-page-row]")].filter((row) => row.dataset.filterHidden !== "true");
  return Math.max(1, Math.ceil(rows.length / pageSize));
}

function setupProfileTabs() {
  document.querySelectorAll("[data-tabs]").forEach((tabs) => {
    const links = [...tabs.querySelectorAll("[data-tab-target]")];
    const panels = [...document.querySelectorAll("[data-tab-panel]")];

    links.forEach((link) => {
      link.addEventListener("click", (event) => {
        event.preventDefault();
        const target = link.dataset.tabTarget;
        links.forEach((item) => item.classList.toggle("active", item === link));
        panels.forEach((panel) => panel.classList.toggle("d-none", panel.dataset.tabPanel !== target));
      });
    });
  });
}

function setupFileInputs() {
  document.querySelectorAll("input[type='file'][data-file-label-target]").forEach((input) => {
    input.addEventListener("change", () => {
      const label = document.querySelector(input.dataset.fileLabelTarget);
      if (label) {
        label.textContent = input.files?.[0]?.name || "No file selected";
      }
    });
  });
}

function setupDownloadButtons() {
  document.querySelectorAll("[data-download-text]").forEach((button) => {
    button.addEventListener("click", () => {
      const blob = new Blob([button.dataset.downloadText || ""], { type: "text/plain;charset=utf-8" });
      const url = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = button.dataset.downloadFile || "download.txt";
      document.body.appendChild(link);
      link.click();
      link.remove();
      URL.revokeObjectURL(url);
    });
  });

  document.querySelectorAll("[data-export-csv]").forEach((button) => {
    if (button.dataset.exportBound === "true") {
      return;
    }

    button.dataset.exportBound = "true";
    button.addEventListener("click", () => {
      const scope = button.closest("[data-filter-scope]") || document.querySelector("[data-filter-scope]");
      if (scope) {
        exportVisibleRows(scope, button.dataset.exportCsv || "export.csv");
      }
    });
  });
}
