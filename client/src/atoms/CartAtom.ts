import { atom } from "jotai";
import { CartModel } from "../model/CartModel";

const initialCart = localStorage.getItem('cartItems');
const parsedCart: CartModel = initialCart
  ? JSON.parse(initialCart)
  : { customerId: 0, orderEntries: [] };

export const CartAtom = atom<CartModel>(parsedCart);
