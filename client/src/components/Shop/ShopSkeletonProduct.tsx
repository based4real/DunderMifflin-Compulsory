export default function ShopSkeletonProduct() {

    return (
        <div className="border-base-300 bg-base-100 border shadow-lg rounded-box p-6 flex flex-col justify-between h-full skeleton">
            <div className="flex justify-between items-start">
                <div className="space-y-7 flex-grow">
                    <div className="space-y-3">
                        <div className="h-5 w-1/2 bg-gray-300 rounded"></div>
                        <div className="h-4 w-3/4 bg-gray-300 rounded"></div>
                    </div>
                    <div className="h-4 w-1/4 bg-gray-300 rounded mt-2"></div>
                </div>
                <div className="flex flex-col items-end space-y-3">
                    <div className="h-7 w-16 bg-gray-300 rounded"></div>
                    <div className="flex gap-4 flex-row">
                        <div className="h-12 w-24 bg-gray-300 rounded"></div>
                        <div className="h-12 w-32 bg-gray-300 rounded"></div>
                    </div>
                </div>
            </div>
        </div>
    );
};