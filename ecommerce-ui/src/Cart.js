import React from "react";

export default function Cart({ cart, total, onRemove }) {
  return (
    <div style={{ padding: "20px" }}>
      <h2>Cart</h2>

      {cart.length === 0 ? (
        <p>Cart is empty</p>
      ) : (
        cart.map((item) => (
          <div key={item.id}>
            {item.name} (x{item.quantity}) - ₹
            {item.price * item.quantity}

            <button onClick={() => onRemove(item.id)}>
              Remove
            </button>
          </div>
        ))
      )}

      <h3>Total: ₹{total}</h3>
    </div>
  );
}