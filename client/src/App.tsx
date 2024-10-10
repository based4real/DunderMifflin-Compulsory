import React, {useEffect} from "react";
import {useAtom} from "jotai";
import {ThemeAtom} from "./atoms/ThemeAtom";
import { Routes, Route } from 'react-router-dom';
import Navigation from "./components/Navigation/Navigation";
import DaisyToaster from "./components/Feedback/DaisyToaster";
import CheckBackend from "./components/Feedback/CheckBackend";
import CartUpdater from './components/Cart/CartUpdater';
import HomePage from "./pages/Home";
import ShopPage from "./pages/Shop";
import CartPage from "./pages/Cart";
import AdminProductsPage from "./pages/Admin/Products";
import OrderPage from "./pages/Order";
import AdminCustomersPage from "./pages/Admin/Customers";

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
      <CartUpdater />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/shop" element={<ShopPage />} />
        <Route path="/shop/cart" element={<CartPage />} />
        <Route path="/order/success" element={<OrderPage />} />

        <Route path="/admin/products" element={<AdminProductsPage />} />
        <Route path="/admin/customers" element={<AdminCustomersPage /> } />
      </Routes>
      <CheckBackend />
      </>
  );
};

export default App;
