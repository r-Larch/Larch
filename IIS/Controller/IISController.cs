using System.Collections.Generic;
using System.Linq;
using System.Net;
using LarchConsole;
using Microsoft.Web.Administration;


namespace IIS.Controller {
    public class IisController {
        private readonly ServerManager _iisManager;

        public IisController(ServerManager iisManager) {
            _iisManager = iisManager;
        }

        public void List(Filter filter, FilterProp what) {
            var filterd = Filter(filter, what);

            using (new Watch("print")) {
                ConsoleEx.PrintWithPaging(
                    list: filterd,
                    countAll: _iisManager.Sites.Count,
                    header: ConsoleWriter.CreateLine("   ID │"),
                    line: (x, i) => new ConsoleWriter()
                        .FormatLine("{id,5} │ {state,-9} {name}", p => p
                            .Add("id", x.Id, what == FilterProp.Id)
                            .Add("state", x.State, what == FilterProp.State)
                            .Add("name", x.Name, what == FilterProp.Name)
                        )
                        .FormatLines("      │ {schema}: {ip}:{port,-4} {host}", x.Bindings, (b, p) => p
                            .Add("schema", b.Model.Schema.Name)
                            .Add("ip", GetAddress(b.Model.EndPoint))
                            .Add("port", GetPort(b.Model.EndPoint))
                            .Add("host", b, what == FilterProp.Binding)
                        )
                        .WriteLine()
                    );
            }
        }

        public void Remove(Filter filter, FilterProp what, bool force) {
            var filterd = Filter(filter, what);
            throw new System.NotImplementedException();
        }

        public void Add(string value, FilterProp what) {
            throw new System.NotImplementedException();
        }

        private IEnumerable<IisSite> Filter(Filter filter, FilterProp what) {
            using (new Watch("filter")) {
                var temp = _iisManager.Sites.Select(x => new IisSite() {
                    Name = filter.GetMatch(x.Name),
                    Id = filter.GetMatch(x.Id),
                    State = filter.GetMatch(x.State),
                    Bindings = x.Bindings.Select(_ => filter.GetMatch(_, binding => binding.Host)).ToList(),
                });

                switch (what) {
                    default:
                    case FilterProp.Binding:
                        return temp.Where(x => x.Bindings.Any(_ => _.IsSuccess));
                    case FilterProp.Name:
                        return temp.Where(x => x.Name.IsSuccess);
                    case FilterProp.Id:
                        return temp.Where(x => x.Id.IsSuccess);
                    case FilterProp.State:
                        return temp.Where(x => x.State.IsSuccess);
                }
            }
        }

        private string GetAddress(IPEndPoint endPoint) {
            if (endPoint == null) return "<null>";

            return Equals(endPoint.Address, IPAddress.Any) ? "*" : endPoint.Address.ToString();
        }

        private string GetPort(IPEndPoint endPoint) {
            return endPoint?.Port.ToString() ?? "<null>";
        }
    }


    internal class IisSite {
        public Match<string> Name { get; set; }
        public Match<long> Id { get; set; }
        public Match<ObjectState> State { get; set; }
        public List<Match<Binding>> Bindings { get; set; }
    }
}