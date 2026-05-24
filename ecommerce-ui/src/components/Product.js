import React, { useEffect, useState } from "react";
import axios from "axios";

export default function Product({ onAdd }) {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [search, setSearch] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("");
  const [minPrice, setMinPrice] = useState("");
  const [maxPrice, setMaxPrice] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);

  const API_BASE_URL = "http://localhost:5119/api";
  const pageSize = 12;

  useEffect(() => {
    fetchCategories();
    fetchProducts();
  }, []);

  useEffect(() => {
    fetchProducts();
  }, [search, selectedCategory, minPrice, maxPrice, currentPage]);

  const fetchCategories = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/categories`);
      setCategories(response.data || []);
    } catch (err) {
      console.error("Error fetching categories:", err);
    }
  };

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const params = {
        pageNumber: currentPage,
        pageSize
      };

      if (search) params.search = search;
      if (selectedCategory) params.category = selectedCategory;
      if (minPrice) params.minPrice = minPrice;
      if (maxPrice) params.maxPrice = maxPrice;

      const response = await axios.get(`${API_BASE_URL}/products`, { params });
      setProducts(response.data.products || []);
      setTotalCount(response.data.totalCount || 0);
    } catch (err) {
      console.error("Error fetching products:", err);
      setProducts([]);
    } finally {
      setLoading(false);
    }
  };

  const handleResetFilters = () => {
    setSearch("");
    setSelectedCategory("");
    setMinPrice("");
    setMaxPrice("");
    setCurrentPage(1);
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <div style={{ flex: 1 }}>
      {/* Search and Filter Section */}
      <div style={{
        backgroundColor: "white",
        padding: "20px",
        borderRadius: "8px",
        marginBottom: "20px",
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
      }}>
        <h2 style={{ marginTop: 0 }}>🛍️ Products Catalog</h2>

        {/* Search Bar */}
        <input
          type="text"
          placeholder="Search products..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
            setCurrentPage(1);
          }}
          style={{
            width: "100%",
            padding: "12px",
            marginBottom: "15px",
            borderRadius: "4px",
            border: "1px solid #ddd",
            fontSize: "16px",
            boxSizing: "border-box"
          }}
        />

        {/* Filters */}
        <div style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))",
          gap: "15px",
          marginBottom: "15px"
        }}>
          {/* Category Filter */}
          <select
            value={selectedCategory}
            onChange={(e) => {
              setSelectedCategory(e.target.value);
              setCurrentPage(1);
            }}
            style={{
              padding: "10px",
              borderRadius: "4px",
              border: "1px solid #ddd",
              fontSize: "14px"
            }}
          >
            <option value="">All Categories</option>
            {categories.map(cat => (
              <option key={cat.id} value={cat.name}>
                {cat.name}
              </option>
            ))}
          </select>

          {/* Price Range */}
          <input
            type="number"
            placeholder="Min Price"
            value={minPrice}
            onChange={(e) => {
              setMinPrice(e.target.value);
              setCurrentPage(1);
            }}
            style={{
              padding: "10px",
              borderRadius: "4px",
              border: "1px solid #ddd",
              fontSize: "14px"
            }}
          />

          <input
            type="number"
            placeholder="Max Price"
            value={maxPrice}
            onChange={(e) => {
              setMaxPrice(e.target.value);
              setCurrentPage(1);
            }}
            style={{
              padding: "10px",
              borderRadius: "4px",
              border: "1px solid #ddd",
              fontSize: "14px"
            }}
          />

          <button
            onClick={handleResetFilters}
            style={{
              padding: "10px",
              backgroundColor: "#95a5a6",
              color: "white",
              border: "none",
              borderRadius: "4px",
              cursor: "pointer",
              fontSize: "14px",
              fontWeight: "bold"
            }}
          >
            Reset Filters
          </button>
        </div>

        <p style={{ color: "#666", fontSize: "14px", margin: "10px 0 0 0" }}>
          Showing {products.length} of {totalCount} products
        </p>
      </div>

      {loading && (
        <div style={{ textAlign: "center", padding: "40px" }}>
          <p style={{ fontSize: "18px", color: "#666" }}>Loading products...</p>
        </div>
      )}

      {!loading && products.length === 0 && (
        <div style={{
          backgroundColor: "white",
          padding: "40px",
          textAlign: "center",
          borderRadius: "8px"
        }}>
          <p style={{ fontSize: "18px", color: "#666" }}>No products found</p>
          <button
            onClick={handleResetFilters}
            style={{
              marginTop: "20px",
              padding: "10px 20px",
              backgroundColor: "#3498db",
              color: "white",
              border: "none",
              cursor: "pointer",
              borderRadius: "4px"
            }}
          >
            Clear Filters
          </button>
        </div>
      )}

      {/* Products Grid */}
      {!loading && products.length > 0 && (
        <>
          <div style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(200px, 1fr))",
            gap: "20px",
            marginBottom: "20px"
          }}>
            {products.map((product) => (
              <div
                key={product.id}
                style={{
                  backgroundColor: "white",
                  borderRadius: "8px",
                  overflow: "hidden",
                  boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                  transition: "transform 0.2s",
                  cursor: "pointer"
                }}
                onMouseEnter={(e) => e.currentTarget.style.transform = "translateY(-5px)"}
                onMouseLeave={(e) => e.currentTarget.style.transform = "translateY(0)"}
              >
                {/* Product Image */}
                {product.imageUrl && (
                  <div style={{
                    height: "150px",
                    backgroundColor: "#f0f0f0",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    overflow: "hidden"
                  }}>
                    <img
                      src={product.imageUrl}
                      alt={product.name}
                      style={{
                        maxWidth: "100%",
                        maxHeight: "100%",
                        objectFit: "cover"
                      }}
                    />
                  </div>
                )}

                {/* Product Info */}
                <div style={{ padding: "15px" }}>
                  <h3 style={{ margin: "0 0 10px 0", fontSize: "16px", minHeight: "40px" }}>
                    {product.name}
                  </h3>

                  <p style={{
                    fontSize: "12px",
                    color: "#666",
                    margin: "0 0 10px 0",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap"
                  }}>
                    {product.description}
                  </p>

                  <div style={{
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                    marginBottom: "10px"
                  }}>
                    <span style={{ fontSize: "12px", color: "#999" }}>
                      {product.category}
                    </span>
                    {product.discount && (
                      <span style={{
                        backgroundColor: "#e74c3c",
                        color: "white",
                        padding: "2px 8px",
                        borderRadius: "4px",
                        fontSize: "12px"
                      }}>
                        -{product.discount}%
                      </span>
                    )}
                  </div>

                  <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                    <span style={{ fontSize: "18px", fontWeight: "bold", color: "#2c3e50" }}>
                      ${product.price.toFixed(2)}
                    </span>
                    <span style={{
                      fontSize: "12px",
                      color: product.stock > 0 ? "#27ae60" : "#e74c3c",
                      fontWeight: "bold"
                    }}>
                      {product.stock > 0 ? `${product.stock} in stock` : "Out of stock"}
                    </span>
                  </div>

                  <button
                    onClick={() => onAdd(product)}
                    disabled={product.stock <= 0}
                    style={{
                      width: "100%",
                      padding: "10px",
                      marginTop: "10px",
                      backgroundColor: product.stock > 0 ? "#3498db" : "#bdc3c7",
                      color: "white",
                      border: "none",
                      borderRadius: "4px",
                      cursor: product.stock > 0 ? "pointer" : "not-allowed",
                      fontWeight: "bold",
                      opacity: product.stock > 0 ? 1 : 0.5
                    }}
                  >
                    {product.stock > 0 ? "Add to Cart" : "Out of Stock"}
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div style={{
              display: "flex",
              justifyContent: "center",
              gap: "10px",
              marginTop: "30px",
              flexWrap: "wrap"
            }}>
              <button
                onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                disabled={currentPage === 1}
                style={{
                  padding: "8px 12px",
                  backgroundColor: currentPage === 1 ? "#bdc3c7" : "#3498db",
                  color: "white",
                  border: "none",
                  borderRadius: "4px",
                  cursor: currentPage === 1 ? "not-allowed" : "pointer"
                }}
              >
                Previous
              </button>

              {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
                <button
                  key={page}
                  onClick={() => setCurrentPage(page)}
                  style={{
                    padding: "8px 12px",
                    backgroundColor: currentPage === page ? "#2c3e50" : "#ecf0f1",
                    color: currentPage === page ? "white" : "#2c3e50",
                    border: "none",
                    borderRadius: "4px",
                    cursor: "pointer",
                    fontWeight: currentPage === page ? "bold" : "normal"
                  }}
                >
                  {page}
                </button>
              ))}

              <button
                onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                disabled={currentPage === totalPages}
                style={{
                  padding: "8px 12px",
                  backgroundColor: currentPage === totalPages ? "#bdc3c7" : "#3498db",
                  color: "white",
                  border: "none",
                  borderRadius: "4px",
                  cursor: currentPage === totalPages ? "not-allowed" : "pointer"
                }}
              >
                Next
              </button>
            </div>
          )}
        </>
      )}
    </div>
  );
}