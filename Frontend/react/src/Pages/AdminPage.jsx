import React, { useEffect, useMemo, useState } from "react";
import api from "../lib/api";
import { useAuth } from "../context/AuthContext";

// ---- SHARED STYLES (matchar din stil) ----
const baseStyles = {
  wrap: {
    maxWidth: 1100,
    margin: "40px auto",
    padding: 28,
    background: "linear-gradient(145deg, rgba(255,94,87,0.96), rgba(210,33,18,0.94))",
    borderRadius: 12,
    color: "#fff",
    boxShadow: "0 0 0 4px #ffd166, 0 0 0 10px rgba(0,0,0,0.85), 0 22px 30px rgba(0,0,0,0.45)",
    border: "4px solid #1f3fff",
    fontFamily: '"Press Start 2P", "VT323", "Courier New", monospace',
    letterSpacing: 0.5,
    position: "relative",
    overflow: "hidden",
  },
  badge: {
    position: "absolute",
    top: -22,
    left: "50%",
    transform: "translateX(-50%)",
    backgroundColor: "#ffd166",
    color: "#b3001b",
    padding: "8px 14px",
    borderRadius: 999,
    border: "3px solid #1f3fff",
    boxShadow: "0 6px 0 #1f3fff, 0 9px 16px rgba(0,0,0,0.35)",
    fontSize: 12,
    textTransform: "uppercase",
  },
  title: { textAlign: "center", marginBottom: 24, fontSize: 20, textShadow: "4px 4px 0 rgba(0,0,0,0.35)" },
  tabs: { display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 8, marginBottom: 18 },
  button: {
    padding: "12px 14px",
    backgroundColor: "#7fe18a",
    color: "#064b2d",
    border: "4px solid #0b5c33",
    borderRadius: 10,
    cursor: "pointer",
    fontWeight: "bold",
    textTransform: "uppercase",
    letterSpacing: 1,
    transition: "background 0.2s ease-in-out, transform 0.2s ease, box-shadow 0.2s ease",
    boxShadow: "0 6px 0 #0b5c33, 0 12px 18px rgba(0,0,0,0.45), inset 0 -4px 0 rgba(0,0,0,0.2)",
  },
  tab: (active) => ({
    padding: "10px 8px",
    backgroundColor: active ? "#ffd166" : "#14213d",
    color: active ? "#5b2b00" : "#fff",
    border: "3px solid #ffd166",
    borderRadius: 10,
    cursor: "pointer",
    fontWeight: "bold",
    textTransform: "uppercase",
    letterSpacing: 1,
    boxShadow: active ? "0 6px 0 #c49d2d" : "0 6px 0 #0b5c33",
  }),
  error: {
    color: "#ffef9f",
    backgroundColor: "rgba(0,0,0,0.35)",
    padding: "10px 14px",
    borderRadius: 8,
    textAlign: "center",
    marginBottom: 16,
    border: "2px dashed rgba(255,239,159,0.65)",
    fontSize: 12,
  },
  section: {
    background: "#14213d",
    border: "3px solid #ffd166",
    borderRadius: 12,
    padding: 16,
    color: "#fff",
    boxShadow: "0 10px 18px rgba(0,0,0,0.45)",
    marginBottom: 18,
  },
  grid: { display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))", gap: 12 },
  row: { display: "flex", gap: 12},
  input: {
    width: "92%",
    padding: "10px 12px",
    borderRadius: 8,
    border: "3px solid #ffd166",
    backgroundColor: "#0b1b36",
    color: "#fff",
    fontSize: 13,
    boxShadow: "inset 0 4px 0 rgba(0,0,0,0.3)",
    marginBottom: "10px",
  },
  label: { display: "block", marginBottom: 6, fontSize: 12, textTransform: "uppercase", color: "#ffef9f" },
  small: { fontSize: 11, color: "#ffef9f" },
  danger: {
    padding: "10px 12px",
    backgroundColor: "#ff9aa2",
    color: "#5a0a12",
    border: "4px solid #7a101b",
    borderRadius: 10,
    cursor: "pointer",
    fontWeight: "bold",
    textTransform: "uppercase",
    boxShadow: "0 6px 0 #7a101b, 0 12px 18px rgba(0,0,0,0.45)",
  },
  ghost: {
    padding: "10px 12px",
    backgroundColor: "transparent",
    color: "#ffef9f",
    border: "3px dashed #ffd166",
    borderRadius: 10,
    cursor: "pointer",
    fontWeight: "bold",
  },
  table: { width: "100%", borderCollapse: "separate", borderSpacing: "0 8px"},
  th: { textAlign: "left", fontSize: 12, color: "#ffef9f", padding: "6px 8px" },
  td: { padding: "10px 8px", background: "#0b1b36", border: "2px solid #20345f", fontSize: 12 },
  right: { textAlign: "right" },
};

const fmtDate = (iso) =>
  iso ? new Intl.DateTimeFormat("sv-SE", { dateStyle: "medium", timeStyle: "short" }).format(new Date(iso)) : "";

const toMap = (arr, key = "id", val = "name") =>
  Object.fromEntries((arr || []).map(x => [x[key], x[val]]));

const Select = ({ value, onChange, options, placeholder = "— Välj —" }) => (
  <select style={baseStyles.input} value={value ?? ""} onChange={e => onChange(e.target.value || "")}>
    <option value="">{placeholder}</option>
    {options.map(o => (
      <option key={o.value} value={o.value}>{o.label}</option>
    ))}
  </select>
);

// ---- ÖVERSIKT ----
function Overview() {
  const { ready } = useAuth();
  const [from, setFrom] = useState(() => new Date(Date.now() - 7 * 864e5).toISOString().slice(0, 10));
  const [to, setTo] = useState(() => new Date().toISOString().slice(0, 10));
  const [summary, setSummary] = useState(null);
  const [topActs, setTopActs] = useState([]);
  const [topPlaces, setTopPlaces] = useState([]);
  const [err, setErr] = useState("");

  async function load() {
    setErr("");
    try {
      const q = `?from=${new Date(from).toISOString()}&to=${new Date(to).toISOString()}`;
      const [{ data: s }, { data: a }, { data: p }] = await Promise.all([
        api.get(`/api/Statistics/summary${q}`),
        api.get(`/api/Statistics/top-activities${q}`),
        api.get(`/api/Statistics/top-places${q}`),
      ]);
      setSummary(s);
      setTopActs(a || []);
      setTopPlaces(p || []);
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }

  useEffect(() => {
    if (ready) load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [ready]);

  return (
    <div style={baseStyles.section}>
      <h3 style={{ marginTop: 0 }}>Översikt</h3>
      <div className ="row" style={baseStyles.row}>
        <div style={{ flex: 1 }}>
          <label style={baseStyles.label}>Från</label>
          <input type="date" value={from} onChange={(e) => setFrom(e.target.value)} style={baseStyles.input} />
        </div>
        <div style={{ flex: 1 }}>
          <label style={baseStyles.label}>Till</label>
          <input type="date" value={to} onChange={(e) => setTo(e.target.value)} style={baseStyles.input} />
        </div>
        <div style={{ alignSelf: "end" }}>
          <button style={baseStyles.button} onClick={load}>
            Uppdatera
          </button>
        </div>
      </div>
      {err && <div style={baseStyles.error}>{err}</div>}
      {summary && (
        <>
          <div style={{ ...baseStyles.grid, marginTop: 12 }}>
            <StatCard label="Totala användare" value={summary.totalUsers} />
            <StatCard label="Aktiva aktiviteter" value={summary.activeActivities} />
            <StatCard label="Aktiva platser" value={summary.activePlaces} />
            <StatCard label="Bokningar" value={summary.totalBookings} />
            <StatCard label="Intäkt (est.)" value={`${summary.estimatedRevenue?.toFixed?.(0)} kr`} />
            <StatCard label="Utnyttjande" value={`${summary.avgUtilizationPercent?.toFixed?.(1)}%`} />
            <StatCard label="Avbokningar" value={`${summary.cancellationRatePercent?.toFixed?.(1)}%`} />
            <StatCard label="Slutförda" value={`${summary.completionRatePercent?.toFixed?.(1)}%`} />
          </div>

          <div style={{ display: "grid", gridTemplateColumns: "1fr", gap: 12, marginTop: 16 }}>
            <div className ="row" style={baseStyles.row}>
            <div style={baseStyles.section}>
              <h4 style={{ marginTop: 0 }}>Toppaktiviteter</h4>
              <table style={baseStyles.table}>
                <thead>
                  <tr>
                    <th style={baseStyles.th}>Namn</th>
                    <th style={{ ...baseStyles.th, ...baseStyles.right }}>Antal</th>
                  </tr>
                </thead>              
                <tbody>
                  {topActs.map((x) => (
                    <tr key={x.id}>
                      <td style={baseStyles.td}>{x.name}</td>
                      <td style={{ ...baseStyles.td, ...baseStyles.right }}>{x.count}</td>
                    </tr>
                  ))}
                </tbody>           
              </table>
            </div>
            <div style={baseStyles.section}>
              <h4 style={{ marginTop: 0 }}>Toppplatser</h4>
              <table style={baseStyles.table}>
                <thead>
                  <tr>
                    <th style={baseStyles.th}>Namn</th>
                    <th style={{ ...baseStyles.th, ...baseStyles.right }}>Antal</th>
                  </tr>
                </thead>
                <tbody>
                  {topPlaces.map((x) => (
                    <tr key={x.id}>
                      <td style={baseStyles.td}>{x.name}</td>
                      <td style={{ ...baseStyles.td, ...baseStyles.right }}>{x.count}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
function StatCard({ label, value }) {
  return (
    <div style={baseStyles.section}>
      <div style={baseStyles.small}>{label}</div>
      <div style={{ fontSize: 20, marginTop: 6 }}>{value ?? "-"}</div>
    </div>
  );
}

// ---- AKTIVITETER (CRUD) ----
function Activities() {
  const { ready } = useAuth();
  const [items, setItems] = useState([]);
  const [err, setErr] = useState("");

  // 👇 Nya states för kategorier (till select)
  const [categories, setCategories] = useState([]);
  const categoryOptions = useMemo(
    () => categories.map(c => ({
      value: c.id,
      label: c.name + (c.isActive ? "" : " (inaktiv)")
    })),
    [categories]
  );
  const categoryNameById = useMemo(() => toMap(categories, "id", "name"), [categories]);

  const emptyForm = {
    name: "",
    description: "",
    categoryId: "",
    defaultDurationMinutes: 60,
    price: 0,
    imageUrl: "",
    environment: 0,
    isActive: true,
  };
  const [form, setForm] = useState(emptyForm);
  const [editing, setEditing] = useState(null);

  async function load() {
    setErr("");
    try {
      const [acts, cats] = await Promise.all([
        api.get(`/api/Activity?includeInactive=true`),
        api.get(`/api/Category`) // ändra till din ev. "onlyActive" om du vill filtrera
      ]);
      setItems(acts.data || []);
      setCategories(cats.data || []);
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  useEffect(() => { if (ready) load(); }, [ready]);

  async function save() {
    setErr("");
    try {
      const payload = { ...form, categoryId: form.categoryId || null };
      if (editing) {
        await api.put(`/api/Activity/${editing.id}`, payload);
      } else {
        const { data: created } = await api.post(`/api/Activity`, payload);
        setEditing(created);
      }
      setForm(emptyForm);
      setEditing(null);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  async function edit(item) {
    setEditing(item);
    setForm({
      name: item.name || "",
      description: item.description || "",
      categoryId: item.categoryId || "",
      defaultDurationMinutes: item.defaultDurationMinutes ?? 60,
      price: item.price ?? 0,
      imageUrl: item.imageUrl || "",
      environment: item.environment ?? 0,
      isActive: item.isActive ?? true,
    });
  }
  async function remove(id) {
    if (!confirm("Ta bort aktivitet?")) return;
    setErr("");
    try {
      await api.delete(`/api/Activity/${id}`);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }

  return (
    <div style={baseStyles.section}>
      <h3 style={{ marginTop: 0 }}>Aktiviteter</h3>
      {err && <div style={baseStyles.error}>{err}</div>}

      <div style={{ ...baseStyles.section, background: "#0b1b36" }}>
        <div className ="row" style={baseStyles.row}>
          <Field label="Namn">
            <input style={baseStyles.input} value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          </Field>
          <Field label="Kategori">
            {/* 👇 Dropdown med namn */}
            <Select
              value={form.categoryId}
              onChange={(val) => setForm({ ...form, categoryId: val })}
              options={categoryOptions}
              placeholder="— Välj kategori —"
            />
          </Field>
        </div>
        <div style={baseStyles.row}>
          <Field label="Beskrivning">
            <input style={baseStyles.input} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
          </Field>
        </div>
        <div className ="row" style={baseStyles.row}>
          <Field label="Standardlängd (min)">
            <input
              type="number"
              style={baseStyles.input}
              value={form.defaultDurationMinutes}
              onChange={(e) => setForm({ ...form, defaultDurationMinutes: +e.target.value })}
            />
          </Field>
          <Field label="Pris">
            <input type="number" style={baseStyles.input} value={form.price} onChange={(e) => setForm({ ...form, price: +e.target.value })} />
          </Field>
          <Field label="Miljö (0=Indoor,1=Outdoor)">
            <input
              type="number"
              style={baseStyles.input}
              value={form.environment}
              onChange={(e) => setForm({ ...form, environment: +e.target.value })}
            />
          </Field>
        </div>
        <div className ="row" style={baseStyles.row}>
          <Field label="Bild-URL">
            <input style={baseStyles.input} value={form.imageUrl} onChange={(e) => setForm({ ...form, imageUrl: e.target.value })} />
          </Field>
          <Field label="Aktiv?">
            <select
              style={baseStyles.input}
              value={String(form.isActive)}
              onChange={(e) => setForm({ ...form, isActive: e.target.value === "true" })}
            >
              <option value="true">Ja</option>
              <option value="false">Nej</option>
            </select>
          </Field>
        </div>

        <div style={{ display: "flex", gap: 10, marginTop: 10 }}>
          <button style={baseStyles.button} onClick={save}>
            {editing ? "Spara ändringar" : "Skapa aktivitet"}
          </button>
          {editing && (
            <button
              style={baseStyles.ghost}
              onClick={() => {
                setEditing(null);
                setForm(emptyForm);
              }}
            >
              Avbryt
            </button>
          )}
        </div>
      </div>

      <div style={{ maxHeight: 300, overflowY: "auto"}}>
      <table style={baseStyles.table}>
        <thead>
          <tr>
            <th style={baseStyles.th}>Namn</th>
            <th style={baseStyles.th}>Kategori</th>
            <th style={baseStyles.th}>Längd</th>
            <th style={baseStyles.th}>Pris</th>
            <th style={baseStyles.th}>Miljö</th>
            <th style={{ ...baseStyles.th, ...baseStyles.right }}>Åtgärder</th>
          </tr>
        </thead>
        <tbody>
          {items.map((x) => (
            <tr key={x.id}>
              <td style={baseStyles.td}>{x.name}</td>
              <td style={baseStyles.td}>{x.categoryName || categoryNameById[x.categoryId] || x.categoryId || "-"}</td>
              <td style={baseStyles.td}>{x.defaultDurationMinutes} min</td>
              <td style={baseStyles.td}>{x.price} kr</td>
              <td style={baseStyles.td}>
                {x.environment === 1 ? "Utomhus" : "Inomhus"} {x.isActive ? "" : "· (inaktiv)"}
              </td>
              <td style={{ ...baseStyles.td, ...baseStyles.right }}>
                <button style={baseStyles.ghost} onClick={() => edit(x)}>Redigera</button>{" "}
                <button style={baseStyles.danger} onClick={() => remove(x.id)}>Ta bort</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      </div>
    </div>
  );
}


// ---- PLATSER (CRUD + aktivering) ----
function Places() {
  const { ready } = useAuth();
  const [items, setItems] = useState([]);
  const [err, setErr] = useState("");
  const empty = { name: "", address: "", latitude: "", longitude: "", environment: "", capacity: 0, isActive: true };
  const [form, setForm] = useState(empty);
  const [editing, setEditing] = useState(null);

  async function load() {
    setErr("");
    try {
      const { data } = await api.get(`/api/Place`);
      setItems(data || []);
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  useEffect(() => {
    if (ready) load();
  }, [ready]);

  async function save() {
    setErr("");
    try {
      if (editing) {
        await api.put(`/api/Place/${editing.id}`, {
          ...form,
          capacity: +form.capacity,
          latitude: form.latitude === "" ? null : +form.latitude,
          longitude: form.longitude === "" ? null : +form.longitude,
        });
      } else {
        await api.post(`/api/Place`, {
          ...form,
          capacity: +form.capacity,
          latitude: form.latitude === "" ? null : +form.latitude,
          longitude: form.longitude === "" ? null : +form.longitude,
        });
      }
      setForm(empty);
      setEditing(null);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  async function edit(p) {
    setEditing(p);
    setForm({
      name: p.name || "",
      address: p.address || "",
      latitude: p.latitude ?? "",
      longitude: p.longitude ?? "",
      environment: p.environment || "",
      capacity: p.capacity ?? 0,
      isActive: p.isActive ?? true,
    });
  }
  async function remove(id) {
    if (!confirm("Ta bort plats?")) return;
    setErr("");
    try {
      await api.delete(`/api/Place/${id}`);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  async function toggleActive(p) {
    setErr("");
    try {
      await api.patch(`/api/Place/${p.id}/active/${(!p.isActive).toString()}`);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }

  return (
    <div style={baseStyles.section}>
      <h3 style={{ marginTop: 0 }}>Platser</h3>
      {err && <div style={baseStyles.error}>{err}</div>}

      <div style={{ ...baseStyles.section, background: "#0b1b36" }}>
        <div className ="row" style={baseStyles.row}>
          <Field label="Namn">
            <input style={baseStyles.input} value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          </Field>
          <Field label="Kapacitet">
            <input
              type="number"
              style={baseStyles.input}
              value={form.capacity}
              onChange={(e) => setForm({ ...form, capacity: +e.target.value })}
            />
          </Field>
          <Field label="Aktiv?">
            <select
              style={baseStyles.input}
              value={String(form.isActive)}
              onChange={(e) => setForm({ ...form, isActive: e.target.value === "true" })}
            >
              <option value="true">Ja</option>
              <option value="false">Nej</option>
            </select>
          </Field>
        </div>
        <div style={baseStyles.row}>
          <Field label="Adress">
            <input style={baseStyles.input} value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
          </Field>
        </div>
        <div className ="row" style={baseStyles.row}>
          <Field label="Lat">
            <input style={baseStyles.input} value={form.latitude} onChange={(e) => setForm({ ...form, latitude: e.target.value })} />
          </Field>
          <Field label="Lon">
            <input style={baseStyles.input} value={form.longitude} onChange={(e) => setForm({ ...form, longitude: e.target.value })} />
          </Field>
          <Field label="Miljö (text)">
            <input style={baseStyles.input} value={form.environment} onChange={(e) => setForm({ ...form, environment: e.target.value })} />
          </Field>
        </div>
        <div style={{ display: "flex", gap: 10, marginTop: 10 }}>
          <button style={baseStyles.button} onClick={save}>
            {editing ? "Spara ändringar" : "Skapa plats"}
          </button>
          {editing && (
            <button
              style={baseStyles.ghost}
              onClick={() => {
                setEditing(null);
                setForm(empty);
              }}
            >
              Avbryt
            </button>
          )}
        </div>
      </div>

      <div style={{ maxHeight: 300, overflowY: "auto"}}>
      <table style={baseStyles.table}>
        <thead>
          <tr>
            <th style={baseStyles.th}>Namn</th>
            <th style={baseStyles.th}>Kapacitet</th>
            <th style={baseStyles.th}>Miljö</th>
            <th style={baseStyles.th}>Status</th>
            <th style={{ ...baseStyles.th, ...baseStyles.right }}>Åtgärder</th>
          </tr>
        </thead>
        <tbody>
          {items.map((p) => (
            <tr key={p.id}>
              <td style={baseStyles.td}>{p.name}</td>
              <td style={baseStyles.td}>{p.capacity}</td>
              <td style={baseStyles.td}>{p.environment || "-"}</td>
              <td style={baseStyles.td}>{p.isActive ? "Aktiv" : "Inaktiv"}</td>
              <td style={{ ...baseStyles.td, ...baseStyles.right }}>
                <button style={baseStyles.ghost} onClick={() => edit(p)}>
                  Redigera
                </button>{" "}
                <button style={baseStyles.button} onClick={() => toggleActive(p)}>
                  {p.isActive ? "Inaktivera" : "Aktivera"}
                </button>{" "}
                <button style={baseStyles.danger} onClick={() => remove(p.id)}>
                  Ta bort
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      </div>
    </div>
  );
}

// ---- KATEGORIER (CRUD + aktivering/deaktivering) ----
function Categories() {
  const { ready } = useAuth();
  const [items, setItems] = useState([]);
  const [err, setErr] = useState("");

  const empty = { name: "", description: "", isActive: true };
  const [form, setForm] = useState(empty);
  const [editing, setEditing] = useState(null);

  function pickErr(e) {
    // Plocka ut vettigt felmeddelande oavsett backend-format
    return (
      e?.response?.data?.detail ||
      e?.response?.data?.message ||
      (Array.isArray(e?.response?.data?.errors) && e.response.data.errors.map(x => x.errorMessage || x).join(", ")) ||
      e?.message ||
      "Något gick fel"
    );
  }

  async function load() {
    setErr("");
    try {
      const { data } = await api.get(`/api/Category`);
      setItems(data || []);
    } catch (e) {
      setErr(pickErr(e));
    }
  }

  useEffect(() => {
    if (ready) load();
  }, [ready]);

  async function save() {
    setErr("");
    try {
      if (editing) {
        // Update – alla fält är optional i UpdateDto men vi skickar med aktuella
        await api.put(`/api/Category/${editing.id}`, {
          name: form.name,
          description: form.description,
          isActive: form.isActive,
        });
      } else {
        // Create – CreateDto: name + description
        await api.post(`/api/Category`, {
          name: form.name,
          description: form.description,
        });
      }
      setForm(empty);
      setEditing(null);
      await load();
    } catch (e) {
      setErr(pickErr(e));
    }
  }

  function edit(c) {
    setEditing(c);
    setForm({
      name: c.name || "",
      description: c.description || "",
      isActive: c.isActive ?? true,
    });
  }

  async function remove(id) {
    if (!confirm("Ta bort kategori?")) return;
    setErr("");
    try {
      await api.delete(`/api/Category/${id}`);
      await load();
    } catch (e) {
      setErr(pickErr(e));
    }
  }

  async function toggleActive(c) {
  setErr("");
  try {
    await api.patch(`/api/Category/${c.id}/active/${(!c.isActive).toString()}`);
    await load();
  } catch (e) {
    setErr(pickErr(e));
  }
}

  return (
    <div style={baseStyles.section}>
      <h3 style={{ marginTop: 0 }}>Kategorier</h3>
      {err && <div style={baseStyles.error}>{err}</div>}

      <div style={{ ...baseStyles.section, background: "#0b1b36" }}>
        <div className ="row" style={baseStyles.row}>
          <Field label="Namn">
            <input
              style={baseStyles.input}
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              placeholder="t.ex. Padel"
            />
          </Field>
          <Field label="Aktiv?">
            <select
              style={baseStyles.input}
              value={String(form.isActive)}
              onChange={(e) => setForm({ ...form, isActive: e.target.value === "true" })}
              disabled={!editing} // vid create styrs isActive i backend (default true)
            >
              <option value="true">Ja</option>
              <option value="false">Nej</option>
            </select>
          </Field>
        </div>
        <div style={baseStyles.row}>
          <Field label="Beskrivning">
            <input
              style={baseStyles.input}
              value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              placeholder="valfritt"
            />
          </Field>
        </div>

        <div style={{ display: "flex", gap: 10, marginTop: 10 }}>
          <button style={baseStyles.button} onClick={save}>
            {editing ? "Spara ändringar" : "Skapa kategori"}
          </button>
          {editing && (
            <button
              style={baseStyles.ghost}
              onClick={() => {
                setEditing(null);
                setForm(empty);
              }}
            >
              Avbryt
            </button>
          )}
        </div>
      </div>
      
      <div style={{ maxHeight: 300, overflowY: "auto"}}>
      <table style={baseStyles.table}>
        <thead>
          <tr>
            <th style={baseStyles.th}>Namn</th>
            <th style={baseStyles.th}>Beskrivning</th>
            <th style={baseStyles.th}>Status</th>
            <th style={{ ...baseStyles.th, ...baseStyles.right }}>Åtgärder</th>
          </tr>
        </thead>
        <tbody>
          {items.map((c) => (
            <tr key={c.id}>
              <td style={baseStyles.td}>{c.name}</td>
              <td style={baseStyles.td}>{c.description || "-"}</td>
              <td style={baseStyles.td}>{c.isActive ? "Aktiv" : "Inaktiv"}</td>
              <td style={{ ...baseStyles.td, ...baseStyles.right }}>
                <button style={baseStyles.ghost} onClick={() => edit(c)}>
                  Redigera
                </button>{" "}
                <button style={baseStyles.button} onClick={() => toggleActive(c)}>
                  {c.isActive ? "Inaktivera" : "Aktivera"}
                </button>{" "}
                <button style={baseStyles.danger} onClick={() => remove(c.id)}>
                  Ta bort
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      </div>
    </div>
  );
}


// ---- TILLFÄLLEN (CRUD) ----
function Occurrences() {
  const { ready } = useAuth();
  const [items, setItems] = useState([]);
  const [err, setErr] = useState("");

  // 👇 Nya listor för select + tabell-lookup
  const [activities, setActivities] = useState([]);
  const [places, setPlaces] = useState([]);

  const activityOptions = useMemo(
    () => activities.map(a => ({
      value: a.id,
      label: a.name + (a.isActive ? "" : " (inaktiv)")
    })), [activities]
  );
  const placeOptions = useMemo(
    () => places.map(p => ({
      value: p.id,
      label: p.name + (p.isActive ? "" : " (inaktiv)")
    })), [places]
  );

  const activityNameById = useMemo(() => toMap(activities, "id", "name"), [activities]);
  const placeNameById    = useMemo(() => toMap(places, "id", "name"), [places]);

  const [form, setForm] = useState({
    id: "",
    activityId: "",
    placeId: "",
    startUtc: "",
    endUtc: "",
    capacityOverride: "",
    priceOverride: "",
  });
  const [editing, setEditing] = useState(false);

  async function load() {
    setErr("");
    try {
      const [occ, acts, pls] = await Promise.all([
        api.get(`/api/ActivityOccurrence`),
        api.get(`/api/Activity?includeInactive=true`),
        api.get(`/api/Place`)
      ]);
      setItems(occ.data || []);
      setActivities(acts.data || []);
      setPlaces(pls.data || []);
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  useEffect(() => { if (ready) load(); }, [ready]);

  async function save() {
    setErr("");
    try {
      const payload = {
        activityId: form.activityId || null,
        placeId: form.placeId || null,
        startUtc: form.startUtc ? new Date(form.startUtc).toISOString() : null,
        endUtc: form.endUtc ? new Date(form.endUtc).toISOString() : null,
        capacityOverride: form.capacityOverride === "" ? null : +form.capacityOverride,
        priceOverride: form.priceOverride === "" ? null : +form.priceOverride,
      };
      if (editing) {
        await api.put(`/api/ActivityOccurrence`, { ...payload, id: form.id });
      } else {
        await api.post(`/api/ActivityOccurrence`, payload);
      }
      setForm({ id: "", activityId: "", placeId: "", startUtc: "", endUtc: "", capacityOverride: "", priceOverride: "" });
      setEditing(false);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }
  async function edit(o) {
    setEditing(true);
    setForm({
      id: o.id,
      activityId: o.activityId || "",
      placeId: o.placeId || "",
      startUtc: o.startUtc ? new Date(o.startUtc).toISOString().slice(0, 16) : "",
      endUtc: o.endUtc ? new Date(o.endUtc).toISOString().slice(0, 16) : "",
      capacityOverride: o.capacityOverride ?? "",
      priceOverride: o.priceOverride ?? "",
    });
  }
  async function remove(id) {
    if (!confirm("Ta bort tillfälle?")) return;
    setErr("");
    try {
      await api.delete(`/api/ActivityOccurrence/${id}`);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.detail || e.message);
    }
  }

  return (
    <div style={baseStyles.section}>
      <h3 style={{ marginTop: 0 }}>Tillfällen</h3>
      {err && <div style={baseStyles.error}>{err}</div>}

      <div style={{ ...baseStyles.section, background: "#0b1b36" }}>
        <div className="row"style={baseStyles.row}>
          <Field label="Aktivitet">
            {/* 👇 Dropdown med aktivitetsnamn */}
            <Select
              value={form.activityId}
              onChange={(val) => setForm({ ...form, activityId: val })}
              options={activityOptions}
              placeholder="— Välj aktivitet —"
            />
          </Field>
          <Field label="Plats">
            {/* 👇 Dropdown med platsnamn */}
            <Select
              value={form.placeId}
              onChange={(val) => setForm({ ...form, placeId: val })}
              options={placeOptions}
              placeholder="— Välj plats —"
            />
          </Field>
        </div>
        <div className ="row" style={baseStyles.row}>
          <Field label="Start (lokal)">
            <input
              type="datetime-local"
              style={baseStyles.input}
              value={form.startUtc}
              onChange={(e) => setForm({ ...form, startUtc: e.target.value })}
            />
          </Field>
          <Field label="Slut (lokal)">
            <input
              type="datetime-local"
              style={baseStyles.input}
              value={form.endUtc}
              onChange={(e) => setForm({ ...form, endUtc: e.target.value })}
            />
          </Field>
        </div>
        <div className ="row" style={baseStyles.row}>
          <Field label="Capacity override">
            <input
              type="number"
              style={baseStyles.input}
              value={form.capacityOverride}
              onChange={(e) => setForm({ ...form, capacityOverride: e.target.value })}
            />
          </Field>
          <Field label="Price override (kr)">
            <input
              type="number"
              style={baseStyles.input}
              value={form.priceOverride}
              onChange={(e) => setForm({ ...form, priceOverride: e.target.value })}
            />
          </Field>
        </div>
        <div style={{ display: "flex", gap: 10, marginTop: 10 }}>
          <button style={baseStyles.button} onClick={save}>
            {editing ? "Spara ändringar" : "Skapa tillfälle"}
          </button>
          {editing && (
            <button
              style={baseStyles.ghost}
              onClick={() => {
                setEditing(false);
                setForm({ id: "", activityId: "", placeId: "", startUtc: "", endUtc: "", capacityOverride: "", priceOverride: "" });
              }}
            >
              Avbryt
            </button>
          )}
        </div>
      </div>
      <div style={{ maxHeight: 300, overflowY: "auto"}}>
      <table style={baseStyles.table}>
        <thead>
          <tr>
            <th style={baseStyles.th}>Aktivitet</th>
            <th style={baseStyles.th}>Plats</th>
            <th style={baseStyles.th}>Start</th>
            <th style={baseStyles.th}>Slut</th>
            <th style={baseStyles.th}>Cap (eff)</th>
            <th style={{ ...baseStyles.th, ...baseStyles.right }}>Åtgärder</th>
          </tr>
        </thead>
        <tbody>
          {items.map((o) => (
            <tr key={o.id}>
              <td style={baseStyles.td}>{activityNameById[o.activityId] || o.activityName || o.activityId}</td>
              <td style={baseStyles.td}>{placeNameById[o.placeId] || o.placeName || o.placeId}</td>
              <td style={baseStyles.td}>{fmtDate(o.startUtc)}</td>
              <td style={baseStyles.td}>{fmtDate(o.endUtc)}</td>
              <td style={baseStyles.td}>
                {o.effectiveCapacity}
                {o.capacityOverride ? ` (override ${o.capacityOverride})` : ""}
              </td>
              <td style={{ ...baseStyles.td, ...baseStyles.right }}>
                <button style={baseStyles.ghost} onClick={() => edit(o)}>Redigera</button>{" "}
                <button style={baseStyles.danger} onClick={() => remove(o.id)}>Ta bort</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      </div>
    </div>
  );
}


function Field({ label, children }) {
  return (
    <div style={{ flex: 1 }}>
      <label style={baseStyles.label}>{label}</label>
      {children}
    </div>
  );
}

// ---- ADMIN PAGE (flikar) ----
export default function AdminPage() {
  const tabs = useMemo(
    () => [
      { k: "overview", t: "Översikt" },
      { k: "activities", t: "Aktiviteter" },
      { k: "places", t: "Platser" },
      { k: "categories", t: "Kategorier" },
      { k: "occ", t: "Tillfällen" },
    ],
    []
  );
  const [tab, setTab] = useState("overview");

  return (
    <div>
      <style>
      {`
        @media (max-width: 505px) {
          .tabs {
            grid-template-columns: repeat(2, 1fr) !important;
          }
        }

        @media (max-width: 669px) {
      .row {
      display: flex !important;
      flex-direction: column !important;
      gap: 10px !important;
    }
  }
      `}
    </style>
    
      <div style={baseStyles.wrap}>
        <div style={baseStyles.badge}>Admin</div>
        <h2 style={baseStyles.title}>Adminpanel</h2>

        <div className="tabs" style={baseStyles.tabs}>
          {tabs.map((x) => (
            <button key={x.k} style={baseStyles.tab(tab === x.k)} onClick={() => setTab(x.k)}>
              {x.t}
            </button>
          ))}
        </div>

        {tab === "overview" && <Overview />}
        {tab === "activities" && <Activities />}
        {tab === "places" && <Places />}
        {tab === "categories" && <Categories />}
        {tab === "occ" && <Occurrences />}
      </div>
    </div>
  );
}

