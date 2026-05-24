import React from "react";

export default function Cart({ cart, total, onRemove }) {
  return (
    <div className="cart-panel">
      <h2 style={{ marginTop: 0, marginBottom: 8 }}>Your Cart</h2>

      {cart.length === 0 ? (
        <p style={{ color: "#5d6b82" }}>Cart is empty</p>
      ) : (
        <div className="cart-list">
          {cart.map((item) => (
            <div key={item.id} className="cart-item">
              <div className="cart-row" style={{ marginBottom: 6 }}>
                <strong>{item.name}</strong>
                <span>Qty: {item.quantity}</span>
              </div>
              <div className="cart-row">
                <span>Rs {Number(item.price * item.quantity).toFixed(2)}</span>
                <button className="btn btn-danger" onClick={() => onRemove(item.id)}>
                  Remove
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      <h3 className="cart-total">Total: Rs {Number(total).toFixed(2)}</h3>
    </div>
  );
}