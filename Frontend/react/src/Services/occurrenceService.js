import api from "../lib/api.js";

const toLocalNoZ = (dateStr, h = 0, m = 0, s = 0) => {
  const d = new Date(`${dateStr}T00:00:00`);
  d.setHours(h, m, s, 0);
  const pad = (n) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(
    d.getHours()
  )}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
};

const occurrenceService = {
  async list(input = {}) {
    const {
      dateFrom,
      dateTo,
      environment,
      activityId,
      placeId,
      categoryId,
      onlyAvailable,
    } = input;

    const params = {
      ...(dateFrom && { FromDate: toLocalNoZ(dateFrom, 0, 0, 0) }),
      ...(dateTo && { ToDate: toLocalNoZ(dateTo, 23, 59, 59) }),
      ...(environment !== "" && environment !== undefined
        ? { Environment: environment }
        : {}),
      ...(activityId && { ActivityId: activityId }),
      ...(placeId && { PlaceId: placeId }),
      ...(categoryId && { CategoryId: categoryId }),
      ...(typeof onlyAvailable === "boolean"
        ? { OnlyAvailable: onlyAvailable }
        : {}),
    };

    console.log(
      "[occurrenceService] GET /api/ActivityOccurrence/with-weather params:",
      JSON.stringify(params)
    );
    console.log("BASE =", api.defaults.baseURL);
    const res = await api.get("/api/ActivityOccurrence/with-weather", {
      params,
    });
    console.log("URL =", res.request?.responseURL);
    return res.data;
  },
};

export default occurrenceService;
