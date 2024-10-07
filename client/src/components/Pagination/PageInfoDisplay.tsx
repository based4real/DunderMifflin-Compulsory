interface PageInfoDisplayProps {
    currentPage: number;
    pageSize: number;
    totalItems: number;
}

export default function PageInfoDisplay({ currentPage, pageSize, totalItems }: PageInfoDisplayProps) {
    const startItem = totalItems === 0 ? 0 : (currentPage - 1) * pageSize + 1;
    const endItem = Math.min(currentPage * pageSize, totalItems);

    return (
        <div className="text-sm font-semibold mb-4">
            {`Showing ${startItem} to ${endItem} (of ${totalItems} products)`}
        </div>
    );
}