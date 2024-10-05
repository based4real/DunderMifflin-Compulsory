import React, {useEffect} from "react";
import {useAtom} from "jotai";
import {ThemeAtom} from "./atoms/ThemeAtom";
import { Routes, Route } from 'react-router-dom';
import Navigation from "./components/Navigation/Navigation";
import HomePage from "./pages/Home";
import ShopPage from "./pages/Shop";
import CartPage from "./pages/Cart";

const App = () => {

  const [theme, setTheme] = useAtom(ThemeAtom);

  useEffect(() => {
      localStorage.setItem('theme', theme);
      document.documentElement.setAttribute('data-theme', theme);
  }, [theme])

  return (
    <>
    <Navigation/>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/shop" element={<ShopPage />} />
        <Route path="/shop/cart" element={<CartPage />} />
      </Routes>
      </>
  );
};

export default App;
