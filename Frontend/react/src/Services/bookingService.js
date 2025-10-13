import api from "../lib/api.js";

const bookingService = {
  async create({ activityOccurrenceId, people = 1 }) {
    const res = await api.post("/api/bookings", {
      activityOccurrenceId,
      people,
    });
    return res.data;
  },
};

export default bookingService;
