export default function QuantityInput({ max, value, onChange }: { max: number, value: number, onChange: (value: number) => void; }) {
    const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const input = event.target.value;

        if (input === "") {
            onChange(1);
            return;
        }

        const numericValue = Math.max(1, Math.min(Number(input), max));

        onChange(numericValue);
    };

    return (
        <input type="number" 
               disabled={max === 0}
               className="grow w-12" 
               step={1} 
               min={max > 0 ? 1 : 0} 
               max={max} 
               value={max > 0 ? value : 0} 
               onChange={handleQuantityChange}/>
    );
}