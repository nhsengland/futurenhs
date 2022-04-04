import { render, screen } from '@testing-library/react';
import { SiteUserTemplate } from './index';
import { routes } from '@jestMocks/generic-props';
import { Props } from './interfaces';

describe('Site User Template', () => {

    const props: Props = {
        id: "",
        siteUser: {
            id: "string",
            role: "string",
            firstName: "MockFirstName",
            lastName: "MockLastName"
        },
        contentText: {} as any,
        routes: routes
    };

    it('renders correctly', () => {

        render(SiteUserTemplate({ ...props }));

        expect(screen.getAllByText(props.siteUser.firstName).length).toBe(1);
        expect(screen.getAllByText(props.siteUser.lastName).length).toBe(1);
    });

})