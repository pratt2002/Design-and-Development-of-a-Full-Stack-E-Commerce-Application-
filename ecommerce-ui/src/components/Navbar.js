import React from "react";

export default function Navbar({ user, cartCount, onLogout, onAdminAccess }) {
  return (
    <div className="nav-shell">
      <h2 className="brand-title">E-Commerce Store</h2>

      <div className="nav-actions">
        <span>
          Welcome, <strong>{user?.username}</strong>
        </span>

        {user?.role === "Admin" && (
          <button
            onClick={onAdminAccess}
            className="btn btn-accent"
          >
            Admin Dashboard
          </button>
        )}

        <span className="pill-badge">
          {cartCount}
        </span>

        <button 
          onClick={onLogout}
          className="btn btn-danger"
        >
          Logout
        </button>
      </div>
    </div>
  );
}