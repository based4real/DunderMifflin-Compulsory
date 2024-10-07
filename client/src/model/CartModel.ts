import { PaperDetailViewModel } from "../Api";

export interface CartModel {
    customerId: number;
    cartEntries: CartOrderEntry[];
  }
  
  export interface CartOrderEntry {
    paper: PaperDetailViewModel;
    quantity: number;
  }