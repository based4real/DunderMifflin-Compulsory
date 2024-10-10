import { useState } from 'react';
import { toast } from "react-hot-toast";
import { api } from "../../http";
import Modal from "../Modal";
import QuantityInput from "../Input/QuantityInput";

interface RestockProductModalProps {
    isOpen: boolean;
    onClose: () => void;
    productIds: number[];
    productNames: string;
}

export default function RestockProductModal({ isOpen, onClose, productIds, productNames }: RestockProductModalProps) {
    const [amount, setAmount] = useState(1);

    const handleRestock = () => {
        if (productIds.length === 1) {
            api.paper.restock(productIds[0], { amount })
                .then(() => {
                    toast.success(`Restocked ${amount} units of ${productNames}`);
                    onClose();
                })
                .catch(() => {
                    toast.error("Failed to restock the product.");
                });
        } else {
            const bulkRestockData = productIds.map((id) => ({ paperId: id, amount }));
            api.paper.restockBulk(bulkRestockData)
                .then(() => {
                    toast.success(`Restocked ${amount} units for selected products.`);
                    onClose();
                })
                .catch(() => {
                    toast.error("Failed to restock the selected products.");
                });
        }
    };

    if (!isOpen) return null;

    return (
        <Modal title={productIds.length === 1 ? `Restock ${productNames}` : "Restock products"} onClose={onClose}>
            <div className="flex flex-col space-y-4">
                <label className="input input-bordered flex items-center gap-2">
                    <span>Amount</span>
                    <QuantityInput
                        max={2147483647} // int32 max
                        value={amount}
                        onChange={(value) => setAmount(value)}
                    />
                </label>

                <button className="btn btn-primary" onClick={handleRestock}>
                    Restock Products
                </button>
            </div>
        </Modal>
    );
}