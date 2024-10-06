/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface CustomerDetailViewModel {
  /** @format int32 */
  id: number;
  /** @minLength 1 */
  name: string;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
}

export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  /** @format int32 */
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  extensions?: Record<string, any>;
  [key: string]: any;
}

export interface CustomerOrderPagedViewModel {
  customerDetails: CustomerOrderDetailViewModel;
  pagingInfo: PagingInfo;
}

export type CustomerOrderDetailViewModel = CustomerDetailViewModel & {
  orders: OrderDetailViewModel[];
};

export interface OrderDetailViewModel {
  /** @format int32 */
  id: number;
  status?: string | null;
  /** @format date-time */
  orderDate?: string;
  /** @format date */
  deliveryDate?: string | null;
  /** @format double */
  totalPrice: number;
  entry: OrderEntryDetailViewModel[];
}

export interface OrderEntryDetailViewModel {
  /** @format int32 */
  id: number;
  /** @format int32 */
  productId?: number | null;
  productName?: string | null;
  /** @format int32 */
  quantity: number;
  /** @format double */
  price: number;
  /** @format double */
  totalPrice: number;
}

export interface PagingInfo {
  /** @format int32 */
  totalItems: number;
  /** @format int32 */
  itemsPerPage: number;
  /** @format int32 */
  currentPage: number;
  /** @format int32 */
  totalPages: number;
}

export interface OrderCreateModel {
  /**
   * @format int32
   * @min 1
   * @max 2147483647
   */
  customerId: number;
  orderEntries?: OrderCreateEntryModel[];
}

export interface OrderCreateEntryModel {
  /**
   * @format int32
   * @min 1
   * @max 2147483647
   */
  quantity: number;
  /**
   * @format int32
   * @min 1
   * @max 2147483647
   */
  productId: number;
}

export enum OrderStatus {
  Pending = "Pending",
  Shipped = "Shipped",
  Delivered = "Delivered",
}

export interface PaperDetailViewModel {
  /** @format int32 */
  id: number;
  name?: string | null;
  discontinued: boolean;
  /** @format int32 */
  stock: number;
  /** @format double */
  price: number;
  properties?: PaperPropertyDetailViewModel[] | null;
}

export interface PaperPropertyDetailViewModel {
  /** @format int32 */
  id: number;
  name?: string | null;
  paperPropertyDetails?: PaperDetailViewModel[] | null;
}

export interface PaperCreateModel {
  /**
   * @minLength 2
   * @maxLength 255
   */
  name: string;
  /**
   * @format int32
   * @min 0
   * @max 2147483647
   */
  stock?: number;
  /**
   * @format double
   * @min 0.01
   */
  price?: number;
  propertyIds?: number[] | null;
}

export interface PaperPropertyCreateModel {
  /** @minLength 2 */
  name: string;
  papersId?: number[] | null;
}

export interface PaperPagedViewModel {
  papers?: PaperDetailViewModel[];
  pagingInfo: PagingInfo;
}

export enum PaperOrderBy {
  Id = "Id",
  Name = "Name",
  Price = "Price",
  Stock = "Stock",
}

export enum SortOrder {
  Asc = "Asc",
  Desc = "Desc",
}

export enum FilterType {
  Or = "Or",
  And = "And",
}

export interface PaperRestockUpdateModel {
  /**
   * @format int32
   * @min 1
   * @max 2147483647
   */
  paperId?: number;
  /**
   * @format int32
   * @min 1
   * @max 2147483647
   */
  amount?: number;
}

export interface PaperPropertySummaryViewModel {
  /** @format int32 */
  id: number;
  name?: string | null;
}

import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, HeadersDefaults, ResponseType } from "axios";
import axios from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseType;
  /** request body */
  body?: unknown;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
  secure?: boolean;
  format?: ResponseType;
}

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public instance: AxiosInstance;
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private secure?: boolean;
  private format?: ResponseType;

  constructor({ securityWorker, secure, format, ...axiosConfig }: ApiConfig<SecurityDataType> = {}) {
    this.instance = axios.create({ ...axiosConfig, baseURL: axiosConfig.baseURL || "http://localhost:5009" });
    this.secure = secure;
    this.format = format;
    this.securityWorker = securityWorker;
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected mergeRequestParams(params1: AxiosRequestConfig, params2?: AxiosRequestConfig): AxiosRequestConfig {
    const method = params1.method || (params2 && params2.method);

    return {
      ...this.instance.defaults,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...((method && this.instance.defaults.headers[method.toLowerCase() as keyof HeadersDefaults]) || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected stringifyFormItem(formItem: unknown) {
    if (typeof formItem === "object" && formItem !== null) {
      return JSON.stringify(formItem);
    } else {
      return `${formItem}`;
    }
  }

  protected createFormData(input: Record<string, unknown>): FormData {
    if (input instanceof FormData) {
      return input;
    }
    return Object.keys(input || {}).reduce((formData, key) => {
      const property = input[key];
      const propertyContent: any[] = property instanceof Array ? property : [property];

      for (const formItem of propertyContent) {
        const isFileType = formItem instanceof Blob || formItem instanceof File;
        formData.append(key, isFileType ? formItem : this.stringifyFormItem(formItem));
      }

      return formData;
    }, new FormData());
  }

  public request = async <T = any, _E = any>({
    secure,
    path,
    type,
    query,
    format,
    body,
    ...params
  }: FullRequestParams): Promise<AxiosResponse<T>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const responseFormat = format || this.format || undefined;

    if (type === ContentType.FormData && body && body !== null && typeof body === "object") {
      body = this.createFormData(body as Record<string, unknown>);
    }

    if (type === ContentType.Text && body && body !== null && typeof body !== "string") {
      body = JSON.stringify(body);
    }

    return this.instance.request({
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type ? { "Content-Type": type } : {}),
      },
      params: query,
      responseType: responseFormat,
      data: body,
      url: path,
    });
  };
}

/**
 * @title Dunder Mifflin API
 * @version v1
 * @baseUrl http://localhost:5009
 *
 * API for Dunder Mifflin paper shop
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  customer = {
    /**
     * No description
     *
     * @tags Customer
     * @name All
     * @summary Retrieves all customers.
     * @request GET:/api/Customer
     */
    all: (
      query?: {
        /**
         * Include order history if true.
         * @default false
         */
        orders?: boolean;
      },
      params: RequestParams = {},
    ) =>
      this.request<CustomerDetailViewModel[], any>({
        path: `/api/Customer`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Customer
     * @name Get
     * @summary Retrieves a customer by ID.
     * @request GET:/api/Customer/{id}
     */
    get: (id: number, params: RequestParams = {}) =>
      this.request<CustomerDetailViewModel, ProblemDetails>({
        path: `/api/Customer/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Customer
     * @name GetCustomerWithOrders
     * @summary Retrieves a paginated list of orders for a specified customer.
     * @request GET:/api/Customer/{id}/Orders
     */
    getCustomerWithOrders: (
      id: number,
      query?: {
        /**
         * The current page (default is 1)
         * @format int32
         * @min 1
         * @max 2147483647
         * @default 1
         */
        page?: number;
        /**
         * The number of orders per page (default is 10)
         * @format int32
         * @min 1
         * @max 1000
         * @default 10
         */
        pageSize?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<CustomerOrderPagedViewModel, ProblemDetails>({
        path: `/api/Customer/${id}/Orders`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Customer
     * @name GetCustomerOrder
     * @summary Retrieves a specific order for a specified customer.
     * @request GET:/api/Customer/{customerId}/Orders/{orderId}
     */
    getCustomerOrder: (customerId: number, orderId: number, params: RequestParams = {}) =>
      this.request<OrderDetailViewModel, ProblemDetails>({
        path: `/api/Customer/${customerId}/Orders/${orderId}`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
  order = {
    /**
     * No description
     *
     * @tags Order
     * @name CreateOrder
     * @summary Creates a new order.
     * @request POST:/api/Order
     */
    createOrder: (data: OrderCreateModel, params: RequestParams = {}) =>
      this.request<OrderDetailViewModel, any>({
        path: `/api/Order`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Order
     * @name UpdateOrderStatus
     * @summary Updates the status of a specific order.
     * @request PATCH:/api/Order/{id}/status
     */
    updateOrderStatus: (
      id: number,
      query?: {
        /** The new status to set, must be a valid `OrderStatus` value. */
        status?: OrderStatus;
      },
      params: RequestParams = {},
    ) =>
      this.request<void, ProblemDetails>({
        path: `/api/Order/${id}/status`,
        method: "PATCH",
        query: query,
        ...params,
      }),

    /**
     * @description - Duplicate IDs are removed. - Negative or zero IDs are rejected with `400 Bad Request`. - If no valid IDs remain, `404 Not Found` is returned. - If you provide a mix of valid and invalid IDs, the status will be updated only for the valid orders.
     *
     * @tags Order
     * @name UpdateOrderStatusBulk
     * @summary Updates the status of multiple orders.
     * @request PATCH:/api/Order/status
     */
    updateOrderStatusBulk: (
      data: number[],
      query?: {
        /** The new status to set, must be a valid `OrderStatus` value. */
        status?: OrderStatus;
      },
      params: RequestParams = {},
    ) =>
      this.request<void, ProblemDetails>({
        path: `/api/Order/status`,
        method: "PATCH",
        query: query,
        body: data,
        type: ContentType.Json,
        ...params,
      }),
  };
  paper = {
    /**
     * No description
     *
     * @tags Paper
     * @name CreatePapers
     * @summary Creates a list of new paper products.
     * @request POST:/api/Paper
     */
    createPapers: (data: PaperCreateModel[], params: RequestParams = {}) =>
      this.request<PaperDetailViewModel[], ProblemDetails>({
        path: `/api/Paper`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Paper
     * @name All
     * @summary Retrieves a paginated list of paper products.
     * @request GET:/api/Paper
     */
    all: (
      query?: {
        /**
         * Page number to retrieve, must be greater than 0. Default is 1.
         * @format int32
         * @min 1
         * @max 2147483647
         * @default 1
         */
        page?: number;
        /**
         * Number of items per page, between 1 and 1000. Default is 10.
         * @format int32
         * @min 1
         * @max 1000
         * @default 10
         */
        pageSize?: number;
        /** Optional search term for paper names (case-insensitive). */
        search?: string | null;
        /** Filter by discontinued status. Null returns all papers. */
        discontinued?: boolean | null;
        /**
         * Field to order results by, default is Id.
         * @default "Id"
         */
        orderBy?: PaperOrderBy;
        /**
         * Sorting direction: ascending or descending. Default is Asc.
         * @default "Asc"
         */
        sortBy?: SortOrder;
        /** Comma-separated property IDs to filter by. */
        filter?: string | null;
        /**
         * Specifies whether all (And) or any (Or) properties should be matched. Default is Or.
         * @default "Or"
         */
        filterType?: FilterType;
      },
      params: RequestParams = {},
    ) =>
      this.request<PaperPagedViewModel, any>({
        path: `/api/Paper`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Paper
     * @name CreatePaperProperty
     * @summary Creates a Paper Property.
     * @request POST:/api/Paper/property
     */
    createPaperProperty: (data: PaperPropertyCreateModel, params: RequestParams = {}) =>
      this.request<PaperPropertyDetailViewModel, any>({
        path: `/api/Paper/property`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Paper
     * @name AllProperties
     * @summary Retrieves a list of all paper properties.
     * @request GET:/api/Paper/property
     */
    allProperties: (params: RequestParams = {}) =>
      this.request<PaperPropertySummaryViewModel[], any>({
        path: `/api/Paper/property`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Paper
     * @name Discontinue
     * @summary Sets a paper product as discontinued.
     * @request PATCH:/api/Paper/{id}/discontinue
     */
    discontinue: (id: number, params: RequestParams = {}) =>
      this.request<void, ProblemDetails>({
        path: `/api/Paper/${id}/discontinue`,
        method: "PATCH",
        ...params,
      }),

    /**
     * @description - Duplicate IDs are removed. - Negative or zero IDs are ignored. - If no valid IDs remain after filtering, an error is returned. - Valid IDs are processed, and invalid IDs are disregarded.
     *
     * @tags Paper
     * @name DiscontinueBulk
     * @summary Sets multiple paper products as discontinued.
     * @request PATCH:/api/Paper/discontinue
     */
    discontinueBulk: (data: number[], params: RequestParams = {}) =>
      this.request<void, ProblemDetails>({
        path: `/api/Paper/discontinue`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Paper
     * @name Restock
     * @summary Restocks a paper product.
     * @request PATCH:/api/Paper/{id}/restock
     */
    restock: (
      id: number,
      query?: {
        /**
         * Amount to add to the current stock, must be greater than 0.
         * @format int32
         * @min 1
         * @max 2147483647
         */
        amount?: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<void, ProblemDetails>({
        path: `/api/Paper/${id}/restock`,
        method: "PATCH",
        query: query,
        ...params,
      }),

    /**
     * @description - Duplicate paper IDs are not allowed. - Negative or zero IDs and amounts are invalid. - Papers that are discontinued cannot be restocked. - Valid requests are processed, invalid ones are disregarded.
     *
     * @tags Paper
     * @name RestockBulk
     * @summary Restocks multiple paper products.
     * @request PATCH:/api/Paper/restock
     */
    restockBulk: (data: PaperRestockUpdateModel[], params: RequestParams = {}) =>
      this.request<void, ProblemDetails>({
        path: `/api/Paper/restock`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        ...params,
      }),
  };
}
