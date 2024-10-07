interface PriceRangeFilterProps {
    minPrice: number | null;
    maxPrice: number | null;
    onPriceRangeChange: (min: number | null, max: number | null) => void;
}

export default function PriceRangeFilter({ minPrice, maxPrice, onPriceRangeChange }: PriceRangeFilterProps) {
    const priceRanges = [
        { label: "$0 - $50", minPrice: 0, maxPrice: 50 },
        { label: "$50 - $100", minPrice: 50, maxPrice: 100 },
        { label: "$100 - $200", minPrice: 100, maxPrice: 200 },
        { label: "$200+", minPrice: 200, maxPrice: null },
    ];

    const handleCheckboxChange = (range: { minPrice: number; maxPrice: number | null }) => {
        const isChecked =
            minPrice === range.minPrice && maxPrice === range.maxPrice;

        if (isChecked) {
            onPriceRangeChange(null, null);
        } else {
            onPriceRangeChange(range.minPrice, range.maxPrice);
        }
    };

    return (
        <div className="form-control mb-4">
            <h3 className="font-semibold mb-2">Price Range</h3>
            {priceRanges.map((range) => (
                <label key={range.label} className="label cursor-pointer">
                    <input
                        type="checkbox"
                        className="checkbox checkbox-primary"
                        checked={minPrice === range.minPrice && maxPrice === range.maxPrice}
                        onChange={() => handleCheckboxChange(range)}
                    />
                    <span className="label-text ml-2">{range.label}</span>
                </label>
            ))}
        </div>
    );
}