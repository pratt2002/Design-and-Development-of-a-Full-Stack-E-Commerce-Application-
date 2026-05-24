import React, { useState } from "react";
import axios from "axios";
import "../App.css";

export default function Login({ onLogin }) {
  const [isRegister, setIsRegister] = useState(false);
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const API_BASE_URL = "http://localhost:5119/api";

  const handleLogin = async () => {
    setError("");
    setLoading(true);

    try {
      if (!username || !password) {
        setError("Username and password are required");
        return;
      }

      const response = await axios.post(`${API_BASE_URL}/auth/login`, {
        username,
        password
      });

      const { token, ...userData } = response.data;
      
      // Store token and user data
      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(userData));
      
      onLogin(userData);
    } catch (err) {
      setError(err.response?.data?.message || "Invalid credentials");
      console.error("Login error:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleRegister = async () => {
    setError("");
    setLoading(true);

    try {
      if (!username || !email || !password) {
        setError("Username, email, and password are required");
        return;
      }

      if (password !== confirmPassword) {
        setError("Passwords do not match");
        return;
      }

      if (password.length < 8) {
        setError("Password must be at least 8 characters");
        return;
      }

      const response = await axios.post(`${API_BASE_URL}/auth/register`, {
        username,
        email,
        password,
        firstName,
        lastName
      });

      const { token, ...userData } = response.data;
      
      // Store token and user data
      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(userData));
      
      onLogin(userData);
    } catch (err) {
      setError(err.response?.data?.message || "Registration failed");
      console.error("Register error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      minHeight: "100vh",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      backgroundColor: "#f5f5f5"
    }}>
      <div style={{
        backgroundColor: "white",
        padding: "40px",
        borderRadius: "8px",
        boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
        width: "100%",
        maxWidth: "400px"
      }}>
        <h2 style={{ textAlign: "center", marginBottom: "30px", color: "#2c3e50" }}>
          {isRegister ? "Create Account" : "Welcome Back"}
        </h2>

        {error && (
          <div style={{
            backgroundColor: "#f8d7da",
            color: "#721c24",
            padding: "12px",
            borderRadius: "4px",
            marginBottom: "20px",
            border: "1px solid #f5c6cb"
          }}>
            {error}
          </div>
        )}

        <div style={{ display: "flex", flexDirection: "column", gap: "15px" }}>
          <input
            type="text"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            style={{
              padding: "12px",
              borderRadius: "4px",
              border: "1px solid #ddd",
              fontSize: "14px"
            }}
          />

          {isRegister && (
            <>
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                style={{
                  padding: "12px",
                  borderRadius: "4px",
                  border: "1px solid #ddd",
                  fontSize: "14px"
                }}
              />

              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "10px" }}>
                <input
                  type="text"
                  placeholder="First Name"
                  value={firstName}
                  onChange={(e) => setFirstName(e.target.value)}
                  style={{
                    padding: "12px",
                    borderRadius: "4px",
                    border: "1px solid #ddd",
                    fontSize: "14px"
                  }}
                />

                <input
                  type="text"
                  placeholder="Last Name"
                  value={lastName}
                  onChange={(e) => setLastName(e.target.value)}
                  style={{
                    padding: "12px",
                    borderRadius: "4px",
                    border: "1px solid #ddd",
                    fontSize: "14px"
                  }}
                />
              </div>
            </>
          )}

          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{
              padding: "12px",
              borderRadius: "4px",
              border: "1px solid #ddd",
              fontSize: "14px"
            }}
          />

          {isRegister && (
            <input
              type="password"
              placeholder="Confirm Password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              style={{
                padding: "12px",
                borderRadius: "4px",
                border: "1px solid #ddd",
                fontSize: "14px"
              }}
            />
          )}

          <button
            onClick={isRegister ? handleRegister : handleLogin}
            disabled={loading}
            style={{
              padding: "12px",
              backgroundColor: "#3498db",
              color: "white",
              border: "none",
              borderRadius: "4px",
              cursor: "pointer",
              fontSize: "16px",
              fontWeight: "bold",
              opacity: loading ? 0.6 : 1
            }}
          >
            {loading ? "Loading..." : (isRegister ? "Create Account" : "Login")}
          </button>

          <div style={{
            textAlign: "center",
            marginTop: "20px",
            paddingTop: "20px",
            borderTop: "1px solid #eee"
          }}>
            <p style={{ color: "#666", marginBottom: "10px" }}>
              {isRegister ? "Already have an account?" : "Don't have an account?"}
            </p>
            <button
              onClick={() => {
                setIsRegister(!isRegister);
                setError("");
                setUsername("");
                setEmail("");
                setPassword("");
                setConfirmPassword("");
              }}
              style={{
                backgroundColor: "transparent",
                color: "#3498db",
                border: "none",
                cursor: "pointer",
                textDecoration: "underline",
                fontSize: "14px"
              }}
            >
              {isRegister ? "Go to Login" : "Register here"}
            </button>
          </div>
        </div>

        <div style={{
          marginTop: "30px",
          padding: "15px",
          backgroundColor: "#f0f0f0",
          borderRadius: "4px",
          fontSize: "12px",
          color: "#666"
        }}>
          <p><strong>Demo Credentials:</strong></p>
          <p>Username: admin | Password: password123</p>
          <p>Username: customer | Password: password123</p>
        </div>
      </div>
    </div>
  );
}