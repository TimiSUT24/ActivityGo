import api from "../lib/api.js";

const bookingService = {
  async create({ activityOccurrenceId, people }) {
    const res = await api.post("/api/bookings", {
      activityOccurrenceId,
      peopleCount: people,
    });
    return res.data;
  },
};

export default bookingService;
