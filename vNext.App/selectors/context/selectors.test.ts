import { selectUser, selectPagination } from './index';
import { GetServerSidePropsContext } from '@appTypes/next';

describe('Selectors', () => {

    const mockContext: Partial<GetServerSidePropsContext> = {
        req: {
            user: {
                id: 1234
            },
            body: {
                pageNumber: 2,
                pageSize: 5  
            }
        }
    };

    it('returns selectUser correctly', () => {

        expect(selectUser(mockContext as any)).toStrictEqual({
            id: 1234
        });

        expect(selectUser({
            req: {}
        } as any)).toBeNull();

    });

    it('returns selectPagination correctly', async () => {
                
        expect(selectPagination(mockContext as any)).toStrictEqual({
            pageNumber: 2,
            pageSize: 5  
        });

        expect(selectPagination({
            req: {}
        } as any)).toStrictEqual({
            pageNumber: 1,
            pageSize: 10  
        });

    });
    
});