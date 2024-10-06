interface PageSizeSelectorProps {
    pageSize: number;
    onPageSizeChange: (size: number) => void;
}

export default function PageSizeSelector({ pageSize, onPageSizeChange }: PageSizeSelectorProps) {
    const handlePageSizeChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        onPageSizeChange(Number(event.target.value));
    };

    return (
        <div className="flex items-center mb-4">
            <label htmlFor="pageSize" className="mr-2 font-semibold">Items per page:</label>
            <select
                id="pageSize"
                value={pageSize}
                onChange={handlePageSizeChange}
                className="select select-bordered">
                {[10, 25, 50, 100].map((size) => (
                    <option key={size} value={size}>
                        {size}
                    </option>
                ))}
            </select>
        </div>
    );
}