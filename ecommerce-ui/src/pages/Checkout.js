import React, { useState } from "react";
import axios from "axios";
import "../App.css";

export default function Checkout({ total, userId, token, onBack, onSuccess }) {
  const [step, setStep] = useState("shipping"); // shipping, payment, confirmation
  const [loading, setLoading] = useState(false);
  const [orderId, setOrderId] = useState(null);
  const [error, setError] = useState("");

  const [shippingData, setShippingData] = useState({
    shippingAddress: ""
  });

  const [paymentData, setPaymentData] = useState({
    cardNumber: "",
    cardholderName: "",
    expiry: "",
    cvv: ""
  });

  const API_BASE_URL = "http://localhost:5119/api";
  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json"
  };

  const handleShippingChange = (e) => {
    const { name, value } = e.target;
    setShippingData(prev => ({ ...prev, [name]: value }));
  };

  const handlePaymentChange = (e) => {
    const { name, value } = e.target;
    setPaymentData(prev => ({ ...prev, [name]: value }));
  };

  const handlePlaceOrder = async () => {
    setError("");
    setLoading(true);

    try {
      if (!shippingData.shippingAddress) {
        setError("Shipping address is required");
        setLoading(false);
        return;
      }

      const response = await axios.post(
        `${API_BASE_URL}/orders/${userId}`,
        shippingData,
        { headers }
      );

      setOrderId(response.data.orderId);
      setStep("payment");
    } catch (err) {
      setError(err.response?.data?.message || "Failed to place order");
      console.error("Order error:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleProcessPayment = async () => {
    setError("");
    setLoading(true);

    try {
      if (!paymentData.cardNumber || !paymentData.cardholderName || !paymentData.expiry || !paymentData.cvv) {
        setError("All payment fields are required");
        setLoading(false);
        return;
      }

      const response = await axios.post(
        `${API_BASE_URL}/payment/process`,
        {
          orderId,
          ...paymentData
        },
        { headers }
      );

      setStep("confirmation");
    } catch (err) {
      setError(err.response?.data?.message || "Payment processing failed");
      console.error("Payment error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{
      maxWidth: "600px",
      margin: "0 auto",
      padding: "20px",
      backgroundColor: "white",
      borderRadius: "8px"
    }}>
      <h2>Checkout</h2>

      {error && (
        <div style={{
          backgroundColor: "#f8d7da",
          color: "#721c24",
          padding: "12px",
          borderRadius: "4px",
          marginBottom: "20px"
        }}>
          {error}
        </div>
      )}

      {/* Order Total */}
      <div style={{
        backgroundColor: "#f0f0f0",
        padding: "15px",
        borderRadius: "4px",
        marginBottom: "20px"
      }}>
        <h3 style={{ margin: "0 0 10px 0" }}>Order Total: ${total.toFixed(2)}</h3>
        <p style={{ margin: 0, color: "#666" }}>
          Step {step === "shipping" ? "1" : step === "payment" ? "2" : "3"} of 3
        </p>
      </div>

      {/* Shipping Step */}
      {step === "shipping" && (
        <div style={{ marginBottom: "20px" }}>
          <h3>Shipping Address</h3>
          <textarea
            name="shippingAddress"
            placeholder="Enter your complete shipping address"
            value={shippingData.shippingAddress}
            onChange={handleShippingChange}
            style={{
              width: "100%",
              padding: "12px",
              borderRadius: "4px",
              border: "1px solid #ddd",
              minHeight: "120px",
              fontFamily: "Arial"
            }}
          />
          <div style={{ marginTop: "15px", display: "flex", gap: "10px" }}>
            <button
              onClick={handlePlaceOrder}
              disabled={loading}
              style={{
                flex: 1,
                padding: "12px",
                backgroundColor: "#27ae60",
                color: "white",
                border: "none",
                borderRadius: "4px",
                cursor: "pointer",
                fontWeight: "bold",
                opacity: loading ? 0.6 : 1
              }}
            >
              {loading ? "Processing..." : "Continue to Payment"}
            </button>
            <button
              onClick={onBack}
              style={{
                flex: 1,
                padding: "12px",
                backgroundColor: "#95a5a6",
                color: "white",
                border: "none",
                borderRadius: "4px",
                cursor: "pointer"
              }}
            >
              Back
            </button>
          </div>
        </div>
      )}

      {/* Payment Step */}
      {step === "payment" && orderId && (
        <div style={{ marginBottom: "20px" }}>
          <h3>Payment Information</h3>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "15px" }}>
            <input
              type="text"
              name="cardNumber"
              placeholder="Card Number"
              value={paymentData.cardNumber}
              onChange={handlePaymentChange}
              maxLength="19"
              style={{
                padding: "12px",
                borderRadius: "4px",
                border: "1px solid #ddd",
                gridColumn: "1 / -1"
              }}
            />
            <input
              type="text"
              name="cardholderName"
              placeholder="Cardholder Name"
              value={paymentData.cardholderName}
              onChange={handlePaymentChange}
              style={{
                padding: "12px",
                borderRadius: "4px",
                border: "1px solid #ddd",
                gridColumn: "1 / -1"
              }}
            />
            <input
              type="text"
              name="expiry"
              placeholder="MM/YY"
              value={paymentData.expiry}
              onChange={handlePaymentChange}
              style={{
                padding: "12px",
                borderRadius: "4px",
                border: "1px solid #ddd"
              }}
            />
            <input
              type="text"
              name="cvv"
              placeholder="CVV"
              value={paymentData.cvv}
              onChange={handlePaymentChange}
              maxLength="4"
              style={{
                padding: "12px",
                borderRadius: "4px",
                border: "1px solid #ddd"
              }}
            />
          </div>
          <div style={{
            marginTop: "15px",
            padding: "12px",
            backgroundColor: "#fff3cd",
            borderRadius: "4px",
            color: "#856404",
            fontSize: "14px"
          }}>
            <strong>Demo Card:</strong> 4242 4242 4242 4242 | 12/25 | 123
          </div>
          <div style={{ marginTop: "15px", display: "flex", gap: "10px" }}>
            <button
              onClick={handleProcessPayment}
              disabled={loading}
              style={{
                flex: 1,
                padding: "12px",
                backgroundColor: "#27ae60",
                color: "white",
                border: "none",
                borderRadius: "4px",
                cursor: "pointer",
                fontWeight: "bold",
                opacity: loading ? 0.6 : 1
              }}
            >
              {loading ? "Processing..." : "Complete Payment"}
            </button>
          </div>
        </div>
      )}

      {/* Confirmation Step */}
      {step === "confirmation" && (
        <div style={{
          textAlign: "center",
          padding: "20px",
          backgroundColor: "#d4edda",
          borderRadius: "4px"
        }}>
          <h3 style={{ color: "#155724", margin: "0 0 10px 0" }}>✓ Order Confirmed!</h3>
          <p style={{ color: "#155724", marginBottom: "20px" }}>
            Order #{orderId} has been successfully placed and paid.
          </p>
          <p style={{ color: "#155724", marginBottom: "20px" }}>
            A confirmation email has been sent to your email address.
          </p>
          <button
            onClick={onSuccess}
            style={{
              padding: "12px 30px",
              backgroundColor: "#155724",
              color: "white",
              border: "none",
              borderRadius: "4px",
              cursor: "pointer",
              fontWeight: "bold",
              fontSize: "16px"
            }}
          >
            Continue Shopping
          </button>
        </div>
      )}
    </div>
  );
}