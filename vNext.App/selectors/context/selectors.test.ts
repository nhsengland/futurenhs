import { GetServerSidePropsContext } from '@appTypes/next';

import { routeParams } from '@constants/routes';
import { selectUser, selectParam, selectPagination } from './index';

describe('Selectors', () => {

    const mockContext: Partial<GetServerSidePropsContext> = {
        params: {
            groupId: '1234'
        },
        req: {
            user: {
                id: 1234
            },
            query: {
                pageNumber: 2,
                pageSize: 5  
            }
        }
    };

    it('returns selectParam correctly', () => {

        expect(selectParam(mockContext as any, routeParams.GROUPID)).toBe('1234');

        expect(selectParam({
            req: {}
        } as any, routeParams.GROUPID)).toBeNull();

    });

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
            pageSize: 30  
        });

    });
    
});