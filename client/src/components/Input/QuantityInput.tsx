import { useState } from "react";

export default function QuantityInput({ max }: { max: number }) {
    const [quantity, setQuantity] = useState<string | number>(1);

    const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = event.target.value;

        if (value === "") {
            setQuantity(value);
            return;
        }

        const numericValue = Math.max(1, Math.min(Number(value), max));

        setQuantity(numericValue);
    };

    return (
        <input type="number" 
               className="grow w-12" 
               step={1} 
               min={1} 
               max={max} 
               value={quantity} 
               onChange={handleQuantityChange}
               onBlur={() => { if (quantity === "") setQuantity(1); }} />
    );
}