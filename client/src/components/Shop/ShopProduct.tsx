export default function ShopProduct() {
    return (
      <div className="bg-base-100 shadow-lg rounded-box p-4 flex flex-col h-full">
        <div className="flex justify-between items-start">
          <div className="space-y-4">
            <div className="space-y-2">
              <img src="https://via.placeholder.com/150" alt="Product" className="w-full h-auto" />
              <h3 className="font-bold text-lg">Paper</h3>
              <p className="text-sm text-gray-500">Water Resistant, A4, Bulletproof</p>
            </div>
            <p className="text-sm text-gray-500">500 in stock</p>
          </div>
          <div className="flex flex-col items-end">
          <p className="text-lg font-bold text-primary mb-2 text-center justify-center w-full">$430</p>
          <button className="btn btn-primary">Add to cart</button>
          </div>
        </div>
      </div>
    );
  }
  