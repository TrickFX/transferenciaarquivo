using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RemocaoRegistrosCnab
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string diretorio = @"C:\Arquivos\CNAB";

                string caminhoRelatorio =
                    RemoverOperacoesEParcelasDosArquivos(diretorio);

                Console.WriteLine();
                Console.WriteLine("Processamento concluído com sucesso.");
                Console.WriteLine("Relatório gerado em:");
                Console.WriteLine(caminhoRelatorio);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Erro durante o processamento:");
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para finalizar.");
            Console.ReadKey();
        }

        public static string RemoverOperacoesEParcelasDosArquivos(
            string diretorio)
        {
            // =============================================================
            // CONFIGURAÇÕES DO LAYOUT
            // =============================================================

            /*
             * As posições abaixo são baseadas na linha informada:
             *
             * 0330001372670P...
             *         72670
             *
             * O índice no C# começa em zero.
             */

            // Sequencial nas posições 9 até 13 do arquivo.
            const int indiceSequencial = 8;
            const int tamanhoSequencial = 5;

            // Tipo de registro na posição 8.
            const int indiceTipoRegistro = 7;
            const char tipoRegistroDetalhe = '3';

            // Segmento na posição 14.
            const int indiceSegmento = 13;
            const char segmentoOperacaoParcela = 'P';

            // Operação nas posições 202 até 210.
            const int indiceOperacao = 201;
            const int tamanhoOperacao = 9;

            // Parcela nas posições 218 até 220.
            const int indiceParcela = 217;
            const int tamanhoParcela = 3;

            // As duas últimas linhas não serão verificadas nem alteradas.
            const int quantidadeLinhasFinaisPreservadas = 2;

            // Gera uma cópia do arquivo antes de modificá-lo.
            const bool criarBackup = true;

            // =============================================================
            // LISTA COMPLETA DE OPERAÇÕES E PARCELAS
            // =============================================================

            var operacoesParcelas =
                new HashSet<string>(StringComparer.Ordinal)
                {
                    "204123224|040",
                    "204123224|041",
                    "204123224|042",
                    "204123224|043",
                    "204123224|044",
                    "204123224|045",
                    "204123224|046",
                    "204123224|047",
                    "204123224|048",

                    "204130225|034",
                    "204130225|035",
                    "204130225|036",
                    "204130225|037",
                    "204130225|038",
                    "204130225|039",
                    "204130225|040",
                    "204130225|041",
                    "204130225|042",
                    "204130225|043",
                    "204130225|044",
                    "204130225|045",
                    "204130225|046",
                    "204130225|047",
                    "204130225|048",

                    "204222461|047",
                    "204222461|048",
                    "204222461|049",
                    "204222461|050",
                    "204222461|051",
                    "204222461|052",
                    "204222461|053",
                    "204222461|054",
                    "204222461|055",
                    "204222461|056",
                    "204222461|057",
                    "204222461|058",
                    "204222461|059",
                    "204222461|060",

                    "205105772|052",
                    "205105772|053",
                    "205105772|054",
                    "205105772|055",
                    "205105772|056",
                    "205105772|057",
                    "205105772|058",
                    "205105772|059",
                    "205105772|060",

                    "205210462|026",
                    "205210462|027",
                    "205210462|028",
                    "205210462|029",
                    "205210462|030",
                    "205210462|031",
                    "205210462|032",
                    "205210462|033",
                    "205210462|034",
                    "205210462|035",
                    "205210462|036",
                    "205210462|037",
                    "205210462|038",
                    "205210462|039",
                    "205210462|040",
                    "205210462|041",
                    "205210462|042",
                    "205210462|043",
                    "205210462|044",
                    "205210462|045",
                    "205210462|046",
                    "205210462|047",
                    "205210462|048",

                    "207186981|026",
                    "207186981|027",
                    "207186981|028",
                    "207186981|029",
                    "207186981|030",
                    "207186981|031",
                    "207186981|032",
                    "207186981|033",
                    "207186981|034",
                    "207186981|035",
                    "207186981|036",
                    "207186981|037",
                    "207186981|038",
                    "207186981|039",
                    "207186981|040",
                    "207186981|041",
                    "207186981|042",
                    "207186981|043",
                    "207186981|044",
                    "207186981|045",
                    "207186981|046",
                    "207186981|047",
                    "207186981|048",
                    "207186981|049",
                    "207186981|050",
                    "207186981|051",
                    "207186981|052",
                    "207186981|053",
                    "207186981|054",
                    "207186981|055",
                    "207186981|056",
                    "207186981|057",
                    "207186981|058",
                    "207186981|059",
                    "207186981|060",

                    "208118964|035",
                    "208118964|036",
                    "208118964|037",
                    "208118964|038",
                    "208118964|039",
                    "208118964|040",
                    "208118964|041",
                    "208118964|042",
                    "208118964|043",
                    "208118964|044",
                    "208118964|045",
                    "208118964|046",
                    "208118964|047",
                    "208118964|048",
                    "208118964|049",
                    "208118964|050",
                    "208118964|051",
                    "208118964|052",
                    "208118964|053",
                    "208118964|054",
                    "208118964|055",
                    "208118964|056",
                    "208118964|057",
                    "208118964|058",
                    "208118964|059",
                    "208118964|060",

                    "208513390|065",
                    "208513390|066",
                    "208513390|067",
                    "208513390|068",
                    "208513390|069",
                    "208513390|070",
                    "208513390|071",
                    "208513390|072"
                };

            // =============================================================
            // VALIDAÇÃO DO DIRETÓRIO
            // =============================================================

            if (string.IsNullOrWhiteSpace(diretorio))
            {
                throw new ArgumentException(
                    "O diretório não foi informado.",
                    "diretorio");
            }

            if (!Directory.Exists(diretorio))
            {
                throw new DirectoryNotFoundException(
                    "O diretório não foi encontrado: " + diretorio);
            }

            // =============================================================
            // CONFIGURAÇÃO DO RELATÓRIO
            // =============================================================

            DateTime dataExecucao = DateTime.Now;

            string identificadorExecucao =
                dataExecucao.ToString(
                    "yyyyMMdd_HHmmssfff",
                    CultureInfo.InvariantCulture);

            string caminhoRelatorio =
                Path.Combine(
                    diretorio,
                    "Relatorio_Remocao_CNAB_" +
                    identificadorExecucao +
                    ".txt");

            var relatorio = new List<string>();

            relatorio.Add(
                "======================================================================");

            relatorio.Add(
                "                  RELATÓRIO DE REMOÇÃO DE REGISTROS");

            relatorio.Add(
                "======================================================================");

            relatorio.Add("");

            relatorio.Add(
                "Data da execução : " +
                dataExecucao.ToString(
                    "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Diretório        : " + diretorio);

            relatorio.Add(
                "Registros alvo   : " +
                operacoesParcelas.Count.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Observação       : As duas últimas linhas de cada arquivo");

            relatorio.Add(
                "                   foram preservadas integralmente.");

            relatorio.Add("");

            var operacoesParcelasEncontradas =
                new HashSet<string>(StringComparer.Ordinal);

            int quantidadeArquivosLidos = 0;
            int quantidadeArquivosAlterados = 0;
            int quantidadeTotalLinhasRemovidas = 0;
            int quantidadeTotalSequenciaisAjustados = 0;

            // =============================================================
            // LOCALIZA OS ARQUIVOS
            // =============================================================

            string[] arquivos =
                Directory.GetFiles(
                    diretorio,
                    "*",
                    SearchOption.TopDirectoryOnly);

            Array.Sort(
                arquivos,
                StringComparer.OrdinalIgnoreCase);

            // =============================================================
            // PROCESSAMENTO DOS ARQUIVOS
            // =============================================================

            foreach (string caminhoArquivo in arquivos)
            {
                string nomeArquivo =
                    Path.GetFileName(caminhoArquivo);

                /*
                 * Ignora:
                 *
                 * - backups criados pelo programa;
                 * - relatórios de execuções anteriores;
                 * - o relatório da execução atual.
                 */
                if (nomeArquivo.EndsWith(
                        ".bak",
                        StringComparison.OrdinalIgnoreCase) ||
                    nomeArquivo.StartsWith(
                        "Relatorio_Remocao_CNAB_",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                quantidadeArquivosLidos++;

                try
                {
                    // =====================================================
                    // LEITURA DO ARQUIVO E DETECÇÃO DA CODIFICAÇÃO
                    // =====================================================

                    byte[] bytesArquivo =
                        File.ReadAllBytes(caminhoArquivo);

                    Encoding codificacao;
                    byte[] bomOriginal;
                    int tamanhoBom;

                    if (bytesArquivo.Length >= 4 &&
                        bytesArquivo[0] == 0x00 &&
                        bytesArquivo[1] == 0x00 &&
                        bytesArquivo[2] == 0xFE &&
                        bytesArquivo[3] == 0xFF)
                    {
                        // UTF-32 Big Endian.
                        codificacao =
                            new UTF32Encoding(
                                true,
                                false);

                        bomOriginal =
                            new byte[]
                            {
                                0x00,
                                0x00,
                                0xFE,
                                0xFF
                            };

                        tamanhoBom = 4;
                    }
                    else if (bytesArquivo.Length >= 4 &&
                             bytesArquivo[0] == 0xFF &&
                             bytesArquivo[1] == 0xFE &&
                             bytesArquivo[2] == 0x00 &&
                             bytesArquivo[3] == 0x00)
                    {
                        // UTF-32 Little Endian.
                        codificacao =
                            new UTF32Encoding(
                                false,
                                false);

                        bomOriginal =
                            new byte[]
                            {
                                0xFF,
                                0xFE,
                                0x00,
                                0x00
                            };

                        tamanhoBom = 4;
                    }
                    else if (bytesArquivo.Length >= 3 &&
                             bytesArquivo[0] == 0xEF &&
                             bytesArquivo[1] == 0xBB &&
                             bytesArquivo[2] == 0xBF)
                    {
                        // UTF-8 com BOM.
                        codificacao =
                            new UTF8Encoding(false);

                        bomOriginal =
                            new byte[]
                            {
                                0xEF,
                                0xBB,
                                0xBF
                            };

                        tamanhoBom = 3;
                    }
                    else if (bytesArquivo.Length >= 2 &&
                             bytesArquivo[0] == 0xFE &&
                             bytesArquivo[1] == 0xFF)
                    {
                        // UTF-16 Big Endian.
                        codificacao =
                            new UnicodeEncoding(
                                true,
                                false);

                        bomOriginal =
                            new byte[]
                            {
                                0xFE,
                                0xFF
                            };

                        tamanhoBom = 2;
                    }
                    else if (bytesArquivo.Length >= 2 &&
                             bytesArquivo[0] == 0xFF &&
                             bytesArquivo[1] == 0xFE)
                    {
                        // UTF-16 Little Endian.
                        codificacao =
                            new UnicodeEncoding(
                                false,
                                false);

                        bomOriginal =
                            new byte[]
                            {
                                0xFF,
                                0xFE
                            };

                        tamanhoBom = 2;
                    }
                    else
                    {
                        /*
                         * Arquivos CNAB normalmente utilizam ANSI.
                         *
                         * Para os campos numéricos e espaços, Windows-1252
                         * preserva o conteúdo corretamente.
                         */
                        codificacao =
                            Encoding.GetEncoding(1252);

                        bomOriginal = new byte[0];
                        tamanhoBom = 0;
                    }

                    string conteudoArquivo =
                        codificacao.GetString(
                            bytesArquivo,
                            tamanhoBom,
                            bytesArquivo.Length - tamanhoBom);

                    // =====================================================
                    // IDENTIFICA A QUEBRA DE LINHA ORIGINAL
                    // =====================================================

                    string quebraLinha;

                    if (conteudoArquivo.Contains("\r\n"))
                    {
                        quebraLinha = "\r\n";
                    }
                    else if (conteudoArquivo.Contains("\n"))
                    {
                        quebraLinha = "\n";
                    }
                    else if (conteudoArquivo.Contains("\r"))
                    {
                        quebraLinha = "\r";
                    }
                    else
                    {
                        quebraLinha = Environment.NewLine;
                    }

                    bool arquivoTerminaComQuebraLinha =
                        conteudoArquivo.EndsWith(
                            "\r\n",
                            StringComparison.Ordinal) ||
                        conteudoArquivo.EndsWith(
                            "\n",
                            StringComparison.Ordinal) ||
                        conteudoArquivo.EndsWith(
                            "\r",
                            StringComparison.Ordinal);

                    string[] linhasSeparadas =
                        conteudoArquivo.Split(
                            new[]
                            {
                                "\r\n",
                                "\n",
                                "\r"
                            },
                            StringSplitOptions.None);

                    var linhasOriginais =
                        new List<string>(linhasSeparadas);

                    /*
                     * Quando o arquivo termina com uma quebra de linha,
                     * o Split gera uma posição vazia no final.
                     *
                     * Essa posição não representa uma linha real.
                     */
                    if (arquivoTerminaComQuebraLinha &&
                        linhasOriginais.Count > 0 &&
                        linhasOriginais[
                            linhasOriginais.Count - 1].Length == 0)
                    {
                        linhasOriginais.RemoveAt(
                            linhasOriginais.Count - 1);
                    }

                    /*
                     * Não há linhas processáveis se o arquivo possuir
                     * somente as duas linhas que devem ser preservadas.
                     */
                    if (linhasOriginais.Count <=
                        quantidadeLinhasFinaisPreservadas)
                    {
                        continue;
                    }

                    var novasLinhas =
                        new List<string>(linhasOriginais.Count);

                    var relatorioArquivo =
                        new List<string>();

                    /*
                     * A sequência reinicia por lote.
                     *
                     * A chave utiliza:
                     *
                     * - banco: posições 1 a 3;
                     * - lote: posições 4 a 7.
                     *
                     * Exemplo: 0330001.
                     */
                    var removidosAnteriormentePorLote =
                        new Dictionary<string, int>(
                            StringComparer.Ordinal);

                    int quantidadeRemovidaArquivo = 0;
                    int sequenciaisAjustadosArquivo = 0;

                    int primeiraLinhaFinalPreservada =
                        linhasOriginais.Count -
                        quantidadeLinhasFinaisPreservadas;

                    // =====================================================
                    // PROCESSA CADA LINHA
                    // =====================================================

                    for (int indiceLinha = 0;
                         indiceLinha < linhasOriginais.Count;
                         indiceLinha++)
                    {
                        string linhaOriginal =
                            linhasOriginais[indiceLinha];

                        int numeroLinhaOriginal =
                            indiceLinha + 1;

                        /*
                         * As duas últimas linhas entram no arquivo novo
                         * exatamente como estavam.
                         *
                         * Elas não são:
                         *
                         * - verificadas;
                         * - removidas;
                         * - renumeradas.
                         */
                        if (indiceLinha >=
                            primeiraLinhaFinalPreservada)
                        {
                            novasLinhas.Add(linhaOriginal);
                            continue;
                        }

                        bool ehRegistroDetalhe =
                            linhaOriginal.Length >
                                indiceTipoRegistro &&
                            linhaOriginal[indiceTipoRegistro] ==
                                tipoRegistroDetalhe;

                        bool ehSegmentoP =
                            linhaOriginal.Length >
                                indiceSegmento &&
                            linhaOriginal[indiceSegmento] ==
                                segmentoOperacaoParcela;

                        string chaveLote = string.Empty;

                        if (linhaOriginal.Length >= 7)
                        {
                            chaveLote =
                                linhaOriginal.Substring(0, 7);
                        }

                        int sequencialOriginal = 0;

                        bool possuiSequencialValido =
                            ehRegistroDetalhe &&
                            linhaOriginal.Length >=
                                indiceSequencial +
                                tamanhoSequencial &&
                            int.TryParse(
                                linhaOriginal.Substring(
                                    indiceSequencial,
                                    tamanhoSequencial),
                                NumberStyles.None,
                                CultureInfo.InvariantCulture,
                                out sequencialOriginal);

                        bool possuiTamanhoParaOperacaoParcela =
                            linhaOriginal.Length >=
                            indiceParcela + tamanhoParcela;

                        bool linhaDeveSerRemovida = false;

                        string operacaoEncontrada =
                            string.Empty;

                        string parcelaEncontrada =
                            string.Empty;

                        string chaveEncontrada =
                            string.Empty;

                        /*
                         * A pesquisa é realizada somente em registros
                         * detalhe do segmento P.
                         */
                        if (ehRegistroDetalhe &&
                            ehSegmentoP &&
                            possuiTamanhoParaOperacaoParcela)
                        {
                            operacaoEncontrada =
                                linhaOriginal.Substring(
                                    indiceOperacao,
                                    tamanhoOperacao).Trim();

                            parcelaEncontrada =
                                linhaOriginal.Substring(
                                    indiceParcela,
                                    tamanhoParcela).Trim();

                            /*
                             * Garante que a parcela tenha três posições.
                             *
                             * Exemplo:
                             *
                             * 40 -> 040
                             */
                            parcelaEncontrada =
                                parcelaEncontrada.PadLeft(3, '0');

                            chaveEncontrada =
                                operacaoEncontrada +
                                "|" +
                                parcelaEncontrada;

                            linhaDeveSerRemovida =
                                operacoesParcelas.Contains(
                                    chaveEncontrada);
                        }

                        // =============================================
                        // REMOVE A LINHA LOCALIZADA
                        // =============================================

                        if (linhaDeveSerRemovida)
                        {
                            quantidadeRemovidaArquivo++;

                            operacoesParcelasEncontradas.Add(
                                chaveEncontrada);

                            /*
                             * Cada registro removido reduz em uma posição
                             * os próximos sequenciais do mesmo lote.
                             */
                            if (ehRegistroDetalhe &&
                                !string.IsNullOrEmpty(chaveLote))
                            {
                                int quantidadeRemovidaAnteriormente;

                                if (!removidosAnteriormentePorLote
                                    .TryGetValue(
                                        chaveLote,
                                        out quantidadeRemovidaAnteriormente))
                                {
                                    quantidadeRemovidaAnteriormente = 0;
                                }

                                removidosAnteriormentePorLote[chaveLote] =
                                    quantidadeRemovidaAnteriormente + 1;
                            }

                            relatorioArquivo.Add(
                                "    --------------------------------------------------------------");

                            relatorioArquivo.Add(
                                "    Linha original : " +
                                numeroLinhaOriginal.ToString(
                                    CultureInfo.InvariantCulture));

                            relatorioArquivo.Add(
                                "    Operação       : " +
                                operacaoEncontrada);

                            relatorioArquivo.Add(
                                "    Parcela        : " +
                                parcelaEncontrada);

                            if (possuiSequencialValido)
                            {
                                relatorioArquivo.Add(
                                    "    Sequencial     : " +
                                    sequencialOriginal.ToString(
                                        "00000",
                                        CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                relatorioArquivo.Add(
                                    "    Sequencial     : Não identificado");
                            }

                            relatorioArquivo.Add(
                                "    Conteúdo        : " +
                                linhaOriginal);

                            /*
                             * A linha não é adicionada à coleção
                             * novasLinhas, portanto será removida.
                             */
                            continue;
                        }

                        string linhaNova = linhaOriginal;

                        // =============================================
                        // REGULARIZA O SEQUENCIAL
                        // =============================================

                        if (possuiSequencialValido &&
                            !string.IsNullOrEmpty(chaveLote))
                        {
                            int quantidadeRemovidaAnteriormente;

                            if (removidosAnteriormentePorLote
                                .TryGetValue(
                                    chaveLote,
                                    out quantidadeRemovidaAnteriormente) &&
                                quantidadeRemovidaAnteriormente > 0)
                            {
                                int novoSequencial =
                                    sequencialOriginal -
                                    quantidadeRemovidaAnteriormente;

                                if (novoSequencial < 0)
                                {
                                    throw new InvalidOperationException(
                                        "O novo sequencial ficou negativo. " +
                                        "Arquivo: " +
                                        nomeArquivo +
                                        ". Linha: " +
                                        numeroLinhaOriginal.ToString(
                                            CultureInfo.InvariantCulture) +
                                        ".");
                                }

                                linhaNova =
                                    linhaOriginal.Substring(
                                        0,
                                        indiceSequencial) +

                                    novoSequencial.ToString(
                                        "00000",
                                        CultureInfo.InvariantCulture) +

                                    linhaOriginal.Substring(
                                        indiceSequencial +
                                        tamanhoSequencial);

                                sequenciaisAjustadosArquivo++;
                            }
                        }

                        novasLinhas.Add(linhaNova);
                    }

                    /*
                     * Se nenhuma linha foi encontrada, o arquivo não é
                     * regravado.
                     */
                    if (quantidadeRemovidaArquivo == 0)
                    {
                        continue;
                    }

                    // =====================================================
                    // CRIA O BACKUP
                    // =====================================================

                    string caminhoBackup = string.Empty;

                    if (criarBackup)
                    {
                        caminhoBackup =
                            caminhoArquivo +
                            "." +
                            identificadorExecucao +
                            ".bak";

                        File.Copy(
                            caminhoArquivo,
                            caminhoBackup,
                            false);
                    }

                    // =====================================================
                    // MONTA O NOVO CONTEÚDO
                    // =====================================================

                    string novoConteudo =
                        string.Join(
                            quebraLinha,
                            novasLinhas.ToArray());

                    if (arquivoTerminaComQuebraLinha)
                    {
                        novoConteudo += quebraLinha;
                    }

                    byte[] bytesNovoConteudo =
                        codificacao.GetBytes(novoConteudo);

                    /*
                     * Mantém o BOM original, caso ele exista.
                     */
                    byte[] bytesSaida =
                        new byte[
                            bomOriginal.Length +
                            bytesNovoConteudo.Length];

                    if (bomOriginal.Length > 0)
                    {
                        Buffer.BlockCopy(
                            bomOriginal,
                            0,
                            bytesSaida,
                            0,
                            bomOriginal.Length);
                    }

                    Buffer.BlockCopy(
                        bytesNovoConteudo,
                        0,
                        bytesSaida,
                        bomOriginal.Length,
                        bytesNovoConteudo.Length);

                    // =====================================================
                    // GRAVA O ARQUIVO ALTERADO
                    // =====================================================

                    File.WriteAllBytes(
                        caminhoArquivo,
                        bytesSaida);

                    quantidadeArquivosAlterados++;

                    quantidadeTotalLinhasRemovidas +=
                        quantidadeRemovidaArquivo;

                    quantidadeTotalSequenciaisAjustados +=
                        sequenciaisAjustadosArquivo;

                    // =====================================================
                    // RELATÓRIO DO ARQUIVO
                    // =====================================================

                    relatorio.Add(
                        "----------------------------------------------------------------------");

                    relatorio.Add(
                        "ARQUIVO ALTERADO");

                    relatorio.Add(
                        "----------------------------------------------------------------------");

                    relatorio.Add(
                        "  Nome                 : " +
                        nomeArquivo);

                    relatorio.Add(
                        "  Caminho              : " +
                        caminhoArquivo);

                    if (criarBackup)
                    {
                        relatorio.Add(
                            "  Backup               : " +
                            caminhoBackup);
                    }

                    relatorio.Add(
                        "  Total de linhas      : " +
                        linhasOriginais.Count.ToString(
                            CultureInfo.InvariantCulture));

                    relatorio.Add(
                        "  Linhas removidas      : " +
                        quantidadeRemovidaArquivo.ToString(
                            CultureInfo.InvariantCulture));

                    relatorio.Add(
                        "  Sequenciais ajustados : " +
                        sequenciaisAjustadosArquivo.ToString(
                            CultureInfo.InvariantCulture));

                    relatorio.Add(
                        "  Linhas preservadas    : " +
                        (primeiraLinhaFinalPreservada + 1).ToString(
                            CultureInfo.InvariantCulture) +
                        " e " +
                        linhasOriginais.Count.ToString(
                            CultureInfo.InvariantCulture));

                    relatorio.Add("");

                    relatorio.Add(
                        "  OPERAÇÕES E PARCELAS REMOVIDAS:");

                    relatorio.Add("");

                    relatorio.AddRange(relatorioArquivo);

                    relatorio.Add("");
                }
                catch (Exception ex)
                {
                    relatorio.Add(
                        "----------------------------------------------------------------------");

                    relatorio.Add(
                        "ERRO AO PROCESSAR O ARQUIVO");

                    relatorio.Add(
                        "----------------------------------------------------------------------");

                    relatorio.Add(
                        "  Nome     : " + nomeArquivo);

                    relatorio.Add(
                        "  Caminho  : " + caminhoArquivo);

                    relatorio.Add(
                        "  Erro     : " + ex.Message);

                    relatorio.Add(
                        "  Detalhes : " + ex.ToString());

                    relatorio.Add("");
                }
            }

            // =============================================================
            // IDENTIFICA OS REGISTROS NÃO ENCONTRADOS
            // =============================================================

            var operacoesParcelasNaoEncontradas =
                new List<string>();

            foreach (string chave in operacoesParcelas)
            {
                if (!operacoesParcelasEncontradas.Contains(chave))
                {
                    operacoesParcelasNaoEncontradas.Add(chave);
                }
            }

            operacoesParcelasNaoEncontradas.Sort(
                StringComparer.Ordinal);

            // =============================================================
            // RESUMO FINAL
            // =============================================================

            relatorio.Add(
                "======================================================================");

            relatorio.Add(
                "                         RESUMO DA EXECUÇÃO");

            relatorio.Add(
                "======================================================================");

            relatorio.Add("");

            relatorio.Add(
                "Arquivos lidos           : " +
                quantidadeArquivosLidos.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Arquivos alterados        : " +
                quantidadeArquivosAlterados.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Total de linhas removidas : " +
                quantidadeTotalLinhasRemovidas.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Sequenciais ajustados     : " +
                quantidadeTotalSequenciaisAjustados.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Pares encontrados         : " +
                operacoesParcelasEncontradas.Count.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add(
                "Pares não encontrados     : " +
                operacoesParcelasNaoEncontradas.Count.ToString(
                    CultureInfo.InvariantCulture));

            relatorio.Add("");

            // =============================================================
            // REGISTROS NÃO ENCONTRADOS
            // =============================================================

            if (operacoesParcelasNaoEncontradas.Count > 0)
            {
                relatorio.Add(
                    "OPERAÇÕES E PARCELAS NÃO ENCONTRADAS:");

                relatorio.Add("");

                foreach (
                    string chave in
                    operacoesParcelasNaoEncontradas)
                {
                    string[] partes =
                        chave.Split('|');

                    relatorio.Add(
                        "    Operação: " +
                        partes[0] +
                        " | Parcela: " +
                        partes[1]);
                }

                relatorio.Add("");
            }

            relatorio.Add(
                "======================================================================");

            relatorio.Add(
                "                         FIM DO RELATÓRIO");

            relatorio.Add(
                "======================================================================");

            // =============================================================
            // GRAVA O RELATÓRIO
            // =============================================================

            File.WriteAllLines(
                caminhoRelatorio,
                relatorio.ToArray(),
                new UTF8Encoding(true));

            return caminhoRelatorio;
        }
    }
}
