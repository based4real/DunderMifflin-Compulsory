import React, {useEffect} from "react";
import {useAtom} from "jotai";
import {ThemeAtom} from "./atoms/ThemeAtom";
import { Routes, Route } from 'react-router-dom';
import Navigation from "./components/Navigation/Navigation";
import DaisyToaster from "./components/Feedback/DaisyToaster";
import CheckBackend from "./components/Feedback/CheckBackend";
import HomePage from "./pages/Home";
import ShopPage from "./pages/Shop";
import CartPage from "./pages/Cart";
import AdminProductsPage from "./pages/Admin/Products";

const App = () => {
  const [theme, setTheme] = useAtom(ThemeAtom);

  useEffect(() => {
      localStorage.setItem('theme', theme);
      document.documentElement.setAttribute('data-theme', theme);
  }, [theme])

  return (
    <>
    <Navigation/>
      <DaisyToaster />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/shop" element={<ShopPage />} />
        <Route path="/shop/cart" element={<CartPage />} />

        <Route path="/admin/products" element={<AdminProductsPage />} />
      </Routes>
      <CheckBackend />
      </>
  );
};

export default App;
