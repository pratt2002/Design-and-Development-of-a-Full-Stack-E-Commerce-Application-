import React from "react";

const products = [
  { id: 1, name: "iPhone 15", price: 79999 },
  { id: 2, name: "Samsung S24", price: 69999 },
  { id: 3, name: "OnePlus 12", price: 54999 }
];

export default function Product({ onAdd }) {
  return (
    <div style={{ display: "flex", gap: "20px", padding: "20px" }}>
      {products.map((p) => (
        <div key={p.id} style={{ border: "1px solid #ddd", padding: "10px" }}>
          <h3>{p.name}</h3>
          <p>₹{p.price}</p>

          <button onClick={() => onAdd(p)}>
            Add to Cart
          </button>
        </div>
      ))}
    </div>
  );
}