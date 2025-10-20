// Parsar fel från API-svar (axios-felobjekt) till { message, list }.
// list är en array med strängar (valideringsfel etc.)
// Om okänt fel returneras generisk text.

export function parseApiError(e) {
  const r = e?.response;
  if (!r) return { message: "Nätverksfel eller timeout", list: [] };

  const d = r.data;

  // Strängsvar
  if (typeof d === "string") return { message: d, list: [] };

  // ASP.NET Core ProblemDetails + ModelState
  if (d?.errors && typeof d.errors === "object") {
    const list = Object.entries(d.errors).flatMap(([field, msgs]) =>
      (Array.isArray(msgs) ? msgs : [String(msgs)]).map((m) => `${field}: ${m}`)
    );
    return { message: d.title || d.detail || "Valideringsfel", list };
  }

  // Egen modell: { message, errors: [...] }
  if (Array.isArray(d?.errors))
    return { message: d.message || "Fel", list: d.errors };

  // Vanlig: { message } / { detail } / { title }
  if (d?.message || d?.detail || d?.title) {
    return { message: d.message || d.detail || d.title, list: [] };
  }

  return { message: `Fel ${r.status}`, list: [] };
}
