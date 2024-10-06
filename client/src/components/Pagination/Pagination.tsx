interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
}

export default function Pagination({ currentPage, totalPages, onPageChange }: PaginationProps) {
    const handlePageClick = (page: number) => {
        onPageChange(page);
    };

    return (
        <div className="flex justify-center align-middle mt-4">
            <div className="flex join">
                {Array.from({ length: totalPages }, (_, index) => {
                    const pageNumber = index + 1;
                    return (
                        <button key={pageNumber} 
                                className={`join-item btn ${currentPage === pageNumber ? 'btn-active' : 'bg-base-100'}`} 
                                onClick={() => handlePageClick(pageNumber)}>
                            {pageNumber}
                        </button>
                    );
                })}
            </div>
        </div>
    );
}