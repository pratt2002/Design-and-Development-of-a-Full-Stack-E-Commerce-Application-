import React, { useState, useEffect } from "react";
import axios from "axios";
import Product from "./components/Product";
import Cart from "./components/Cart";
import Navbar from "./components/Navbar";
import Login from "./pages/Login";
import Checkout from "./pages/Checkout";
import AdminDashboard from "./pages/AdminDashboard";

function App() {
  const API_BASE_URL = "http://localhost:5119/api";
  const [user, setUser] = useState(null);
  const [view, setView] = useState("shop");

  const [cart, setCart] = useState(() => {
    return JSON.parse(localStorage.getItem("cart")) || [];
  });

  const getAuthHeaders = () => ({
    Authorization: `Bearer ${localStorage.getItem("token")}`,
    "Content-Type": "application/json"
  });

  const mapServerCartToUi = (cartItems) => {
    return (cartItems || [])
      .filter((item) => item.product)
      .map((item) => ({
        ...item.product,
        id: item.productId,
        quantity: item.quantity,
        price: Number(item.product.price || 0)
      }));
  };

  const syncCartFromServer = async (targetUser = user) => {
    if (!targetUser || targetUser.role === "Admin") return;

    try {
      const response = await axios.get(`${API_BASE_URL}/cart/${targetUser.id}`, {
        headers: getAuthHeaders()
      });
      setCart(mapServerCartToUi(response.data.cartItems));
    } catch (error) {
      console.error("Error syncing cart from server:", error);
    }
  };

  const migrateLocalCartToServer = async (targetUser) => {
    if (!targetUser || targetUser.role === "Admin") return;

    const localCart = JSON.parse(localStorage.getItem("cart") || "[]");
    if (!Array.isArray(localCart) || localCart.length === 0) return;

    try {
      for (const item of localCart) {
        await axios.post(
          `${API_BASE_URL}/cart/${targetUser.id}`,
          {
            productId: item.id,
            quantity: item.quantity || 1
          },
          { headers: getAuthHeaders() }
        );
      }

      localStorage.removeItem("cart");
    } catch (error) {
      console.error("Error migrating local cart:", error);
    }
  };

  // 🔥 FIXED LOGIN HANDLER
  const handleLogin = (userData) => {
    setUser(userData);
    // If admin, redirect to admin dashboard
    if (userData.role === "Admin") {
      setView("admin");
    } else {
      setView("shop");
    }
  };

  useEffect(() => {
    const initializeCart = async () => {
      if (!user || user.role === "Admin") return;
      await migrateLocalCartToServer(user);
      await syncCartFromServer(user);
    };

    initializeCart();
  }, [user]);

  // 🔥 ADD TO CART FIXED (ensures price is number)
  const addToCart = async (product) => {
    if (!user || user.role === "Admin") return;

    try {
      await axios.post(
        `${API_BASE_URL}/cart/${user.id}`,
        {
          productId: product.id,
          quantity: 1
        },
        { headers: getAuthHeaders() }
      );

      await syncCartFromServer(user);
    } catch (error) {
      console.error("Error adding to cart:", error);
    }
  };

  const removeFromCart = async (id) => {
    if (!user || user.role === "Admin") return;

    const existingItem = cart.find((item) => item.id === id);
    if (!existingItem) return;

    try {
      if (existingItem.quantity <= 1) {
        await axios.delete(`${API_BASE_URL}/cart/${user.id}/${id}`, {
          headers: getAuthHeaders()
        });
      } else {
        await axios.patch(
          `${API_BASE_URL}/cart/${user.id}/${id}`,
          { quantity: existingItem.quantity - 1 },
          { headers: getAuthHeaders() }
        );
      }

      await syncCartFromServer(user);
    } catch (error) {
      console.error("Error removing from cart:", error);
    }
  };

  // 🔥 FIXED TOTAL (no NaN ever)
  const total = cart.reduce(
    (sum, i) =>
      sum + Number(i.price || 0) * (i.quantity || 0),
    0
  );

  useEffect(() => {
    localStorage.setItem("cart", JSON.stringify(cart));
  }, [cart]);

  // 🔥 LOGIN SCREEN
  if (!user) {
    return <Login onLogin={handleLogin} />;
  }

  // 🔥 ADMIN VIEW
  if (user.role === "Admin" && view === "admin") {
    return <AdminDashboard user={user} onLogout={() => setUser(null)} />;
  }

  return (
    <div>
      <Navbar
        user={user}
        cartCount={cart.length}
        onLogout={() => {
          setUser(null);
          setCart([]);
          localStorage.removeItem("cart");
          localStorage.removeItem("token");
          localStorage.removeItem("user");
          setView("shop");
        }}
        onAdminAccess={() => user.role === "Admin" && setView("admin")}
      />

      {view === "shop" && (
        <div style={{ display: "flex", gap: "20px", padding: "20px" }}>
          <Product onAdd={addToCart} />

          <div>
            <Cart
              cart={cart}
              total={total}
              onRemove={removeFromCart}
            />

            {cart.length > 0 && (
              <button 
                onClick={() => setView("checkout")}
                style={{
                  padding: "10px 20px",
                  marginTop: "10px",
                  backgroundColor: "#4CAF50",
                  color: "white",
                  border: "none",
                  cursor: "pointer",
                  borderRadius: "4px"
                }}
              >
                Checkout
              </button>
            )}
          </div>
        </div>
      )}

      {view === "checkout" && (
        <Checkout
          total={total}
          userId={user.id}
          token={localStorage.getItem("token")}
          onBack={() => setView("shop")}
          onSuccess={() => {
            setCart([]);
            setView("shop");
          }}
        />
      )}
    </div>
  );
}

export default App;