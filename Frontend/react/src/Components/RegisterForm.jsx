// src/Components/RegisterForm.jsx
import { useState } from "react";
import api from "../lib/api";
import "../CSS/RegisterForm.css";

export default function RegisterForm() {
  const [formData, setFormData] = useState({
    email: "",
    password: "",
    firstname: "",
    lastname: "",
  });
  const [message, setMessage] = useState("");

  const handleChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.email || !formData.password) {
      setMessage("Email and password required");
      return;
    }

    try {
      // ✅ RÄTT PATH: /api/auth/register (små bokstäver)
      const res = await api.post("/api/auth/register", {
        email: formData.email,
        password: formData.password,
        firstName: formData.firstname, // matcha DTO
        lastName: formData.lastname,
      });

      if (res.status === 201) {
        setMessage("Welcome to Mario Kingdom");
      } else {
        setMessage(`Registration completed (${res.status}).`);
      }
    } catch (error) {
      const status = error?.response?.status;
      const serverMsg =
        typeof error?.response?.data === "string"
          ? error.response.data
          : error?.response?.data?.message;

      if (status === 409) setMessage("A user with this email already exists");
      else if (status === 404) setMessage("Endpoint not found. Check base URL & route (/api/auth/register).");
      else setMessage(serverMsg || "Registration failed. Please try again.");
    }
  };

  return (
    <div>
      <form onSubmit={handleSubmit} className="register-form">
        <input
          type="text"
          name="email"
          placeholder="email"
          value={formData.email}
          onChange={handleChange}
          className="register-input"
        />
        <input
          type="password"
          name="password"
          placeholder="password"
          value={formData.password}
          onChange={handleChange}
          className="register-input"
        />
        <input
          type="text"
          name="firstname"
          placeholder="firstname"
          value={formData.firstname}
          onChange={handleChange}
          className="register-input"
        />
        <input
          type="text"
          name="lastname"
          placeholder="lastname"
          value={formData.lastname}
          onChange={handleChange}
          className="register-input"
        />
        <button type="submit" className="register-button">Register</button>
        {message && <p className="register-message">{message}</p>}
      </form>
    </div>
  );
}
